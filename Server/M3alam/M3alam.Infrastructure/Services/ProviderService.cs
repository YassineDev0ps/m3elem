 public class ProviderService : IProviderService
    {
        private readonly ApplicationDbContext _dbContext;

        public ProviderService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProfileDto> GetProfileAsync(Guid providerId)
        {
            var provider = await _dbContext.ServiceProviders
                .Where(p => p.Id == providerId)
                .FirstOrDefaultAsync();

            if (provider == null)
            {
                return null;
            }

            return new ProfileDto
            {
                Id = provider.Id,
                Email = provider.Email,
                FullName = provider.FullName,
                Phone = provider.Phone,
                Skills = provider.Skills,
                ExperienceYears = provider.ExperienceYears,
                Availability = provider.Availability,
                Latitude = provider.Latitude,
                Longitude = provider.Longitude,
                Bio = provider.Bio,
                Rating = provider.Rating,
                TotalJobs = provider.TotalJobs
            };
        }

        public async Task<ProfileDto> UpdateProfileAsync(Guid providerId, UpdateProfileDto updateDto)
        {
            var provider = await _dbContext.ServiceProviders
                .Where(p => p.Id == providerId)
                .FirstOrDefaultAsync();

            if (provider == null)
            {
                return null;
            }

            // Update properties
            provider.FullName = updateDto.FullName;
            provider.Phone = updateDto.Phone;
            provider.Skills = updateDto.Skills;
            provider.ExperienceYears = updateDto.ExperienceYears;
            provider.Availability = updateDto.Availability;
            provider.Latitude = updateDto.Latitude;
            provider.Longitude = updateDto.Longitude;
            provider.Bio = updateDto.Bio;

            await _dbContext.SaveChangesAsync();

            return new ProfileDto
            {
                Id = provider.Id,
                Email = provider.Email,
                FullName = provider.FullName,
                Phone = provider.Phone,
                Skills = provider.Skills,
                ExperienceYears = provider.ExperienceYears,
                Availability = provider.Availability,
                Latitude = provider.Latitude,
                Longitude = provider.Longitude,
                Bio = provider.Bio,
                Rating = provider.Rating,
                TotalJobs = provider.TotalJobs
            };
        }

        public async Task<DashboardDto> GetDashboardAsync(Guid providerId)
        {
            var provider = await _dbContext.ServiceProviders
                .Where(p => p.Id == providerId)
                .FirstOrDefaultAsync();

            if (provider == null)
            {
                return null;
            }

            var pendingRequests = await _dbContext.RequestProviders
                .CountAsync(rp => rp.ProviderId == providerId && rp.Status == RequestProviderStatus.Pending);

            var jobsInProgress = await _dbContext.ServiceRequests
                .CountAsync(r => r.SelectedProviderId == providerId && r.Status == RequestStatus.InProgress);

            return new DashboardDto
            {
                TotalJobsCompleted = provider.TotalJobs,
                PendingRequests = pendingRequests,
                JobsInProgress = jobsInProgress,
                AverageRating = provider.Rating,
                CurrentlyAvailable = provider.Availability
            };
        }

        public async Task<IEnumerable<RequestDto>> GetRequestsAsync(Guid providerId, RequestStatus? status = null)
        {
            // Get provider's location
            var provider = await _dbContext.ServiceProviders
                .Where(p => p.Id == providerId)
                .FirstOrDefaultAsync();

            if (provider == null)
            {
                return Enumerable.Empty<RequestDto>();
            }

            // Get requests where this provider is involved
            var requestProviders = await _dbContext.RequestProviders
                .Where(rp => rp.ProviderId == providerId)
                .Include(rp => rp.Request)
                    .ThenInclude(r => r.Seeker)
                .ToListAsync();

            var requests = requestProviders.Select(rp => rp.Request);

            // Filter by status if provided
            if (status.HasValue)
            {
                requests = requests.Where(r => r.Status == status.Value);
            }

            // Calculate distance for each request
            return requests.Select(r => new RequestDto
            {
                Id = r.Id,
                ServiceType = r.ServiceType,
                Description = r.Description,
                Status = r.Status,
                ScheduledDateTime = r.ScheduledDateTime,
                CreatedAt = r.CreatedAt,
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                DistanceKm = CalculateDistance(provider.Latitude, provider.Longitude, r.Latitude, r.Longitude),
                SeekerId = r.SeekerId,
                SeekerName = r.Seeker?.FullName,
                SeekerPhone = r.Seeker?.Phone,
                ProviderStatus = requestProviders.First(rp => rp.RequestId == r.Id).Status
            }).OrderByDescending(r => r.CreatedAt);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0;
            
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }
        
        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        public async Task<bool> AcceptRequestAsync(Guid providerId, Guid requestId)
        {
            // Get the request provider relationship
            var requestProvider = await _dbContext.RequestProviders
                .Where(rp => rp.ProviderId == providerId && rp.RequestId == requestId)
                .FirstOrDefaultAsync();

            if (requestProvider == null || requestProvider.Status != RequestProviderStatus.Pending)
            {
                return false;
            }

            // Get the request to check status
            var request = await _dbContext.ServiceRequests
                .Where(r => r.Id == requestId)
                .FirstOrDefaultAsync();

            if (request == null || request.Status != RequestStatus.Pending)
            {
                return false;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Update the request provider status
                requestProvider.Status = RequestProviderStatus.Accepted;
                requestProvider.RespondedAt = DateTime.UtcNow;

                // Update the main request
                request.Status = RequestStatus.InProgress;
                request.SelectedProviderId = providerId;
                request.UpdatedAt = DateTime.UtcNow;

                // Decline all other provider requests
                var otherRequests = await _dbContext.RequestProviders
                    .Where(rp => rp.RequestId == requestId && rp.ProviderId != providerId)
                    .ToListAsync();

                foreach (var other in otherRequests)
                {
                    other.Status = RequestProviderStatus.Declined;
                    other.RespondedAt = DateTime.UtcNow;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeclineRequestAsync(Guid providerId, Guid requestId)
        {
            var requestProvider = await _dbContext.RequestProviders
                .Where(rp => rp.ProviderId == providerId && rp.RequestId == requestId)
                .FirstOrDefaultAsync();

            if (requestProvider == null || requestProvider.Status != RequestProviderStatus.Pending)
            {
                return false;
            }

            requestProvider.Status = RequestProviderStatus.Declined;
            requestProvider.RespondedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAvailabilityAsync(Guid providerId, bool availability)
        {
            var provider = await _dbContext.ServiceProviders
                .Where(p => p.Id == providerId)
                .FirstOrDefaultAsync();

            if (provider == null)
            {
                return false;
            }

            provider.Availability = availability;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }