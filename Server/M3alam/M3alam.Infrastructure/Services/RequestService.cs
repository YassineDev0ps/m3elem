using Microsoft.EntityFrameworkCore;
using M3alem.Domain.Entities;
using M3alem.Infrastructure.Data;
using M3alem.Services.Request.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace M3alem.Services.Request.Services
{
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _dbContext;

        public RequestService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateRequestAsync(Guid seekerId, CreateRequestDto createDto)
        {
            // Validate that the seeker exists
            var seeker = await _dbContext.ServiceSeekers
                .Where(s => s.Id == seekerId)
                .FirstOrDefaultAsync();

            if (seeker == null)
            {
                throw new ArgumentException("Invalid seeker ID");
            }

            // Validate that the seeker doesn't have an active request for the same service type
            var existingActiveRequest = await _dbContext.ServiceRequests
                .AnyAsync(r => r.SeekerId == seekerId && 
                              r.ServiceType == createDto.ServiceType && 
                              (r.Status == RequestStatus.Pending || r.Status == RequestStatus.InProgress));

            if (existingActiveRequest)
            {
                throw new InvalidOperationException("You already have an active request for this service type");
            }

            // Validate providers exist and are available
            var selectedProviders = await _dbContext.ServiceProviders
                .Where(p => createDto.SelectedProviderIds.Contains(p.Id) && p.Availability)
                .ToListAsync();

            if (selectedProviders.Count != createDto.SelectedProviderIds.Count)
            {
                throw new InvalidOperationException("One or more selected providers are not available");
            }

            // Create the service request
            var serviceRequest = new ServiceRequest
            {
                Id = Guid.NewGuid(),
                SeekerId = seekerId,
                ServiceType = createDto.ServiceType,
                Description = createDto.Description,
                Latitude = createDto.Latitude,
                Longitude = createDto.Longitude,
                Status = RequestStatus.Pending,
                ScheduledDateTime = createDto.ScheduledDateTime,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.ServiceRequests.Add(serviceRequest);

            // Create request-provider relationships
            foreach (var providerId in createDto.SelectedProviderIds)
            {
                var requestProvider = new RequestProvider
                {
                    Id = Guid.NewGuid(),
                    RequestId = serviceRequest.Id,
                    ProviderId = providerId,
                    Status = RequestProviderStatus.Pending
                };

                _dbContext.RequestProviders.Add(requestProvider);
            }

            await _dbContext.SaveChangesAsync();

            return serviceRequest.Id;
        }

        public async Task<RequestDetailDto> GetRequestDetailAsync(Guid requestId)
        {
            var request = await _dbContext.ServiceRequests
                .Where(r => r.Id == requestId)
                .Include(r => r.Seeker)
                .Include(r => r.SelectedProvider)
                .Include(r => r.RequestProviders)
                    .ThenInclude(rp => rp.Provider)
                .Include(r => r.Rating)
                .FirstOrDefaultAsync();

            if (request == null)
            {
                return null;
            }

            return new RequestDetailDto
            {
                Id = request.Id,
                ServiceType = request.ServiceType,
                Description = request.Description,
                Status = request.Status,
                ScheduledDateTime = request.ScheduledDateTime,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,
                SeekerId = request.SeekerId,
                SeekerName = request.Seeker?.FullName,
                SeekerPhone = request.Seeker?.Phone,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                SelectedProviderId = request.SelectedProviderId,
                SelectedProviderName = request.SelectedProvider?.FullName,
                SelectedProviderPhone = request.SelectedProvider?.Phone,
                SelectedProviderRating = request.SelectedProvider?.Rating,
                Providers = request.RequestProviders.Select(rp => new RequestProviderDto
                {
                    ProviderId = rp.ProviderId,
                    ProviderName = rp.Provider?.FullName,
                    ProviderPhone = rp.Provider?.Phone,
                    ProviderRating = rp.Provider?.Rating ?? 0,
                    Status = rp.Status,
                    RespondedAt = rp.RespondedAt
                }).ToList(),
                Rating = request.Rating != null ? new RatingDto
                {
                    Stars = request.Rating.Stars,
                    Comment = request.Rating.Comment,
                    CreatedAt = request.Rating.CreatedAt
                } : null
            };
        }

        public async Task<bool> MarkRequestCompletedAsync(Guid requestId)
        {
            var request = await _dbContext.ServiceRequests
                .Where(r => r.Id == requestId && r.Status == RequestStatus.InProgress)
                .FirstOrDefaultAsync();

            if (request == null)
            {
                return false;
            }

            // Update status
            request.Status = RequestStatus.Done;
            request.UpdatedAt = DateTime.UtcNow;

            // Update provider's total jobs count
            if (request.SelectedProviderId.HasValue)
            {
                var provider = await _dbContext.ServiceProviders
                    .Where(p => p.Id == request.SelectedProviderId.Value)
                    .FirstOrDefaultAsync();

                if (provider != null)
                {
                    provider.TotalJobs++;
                }
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExpireOldPendingRequestsAsync()
        {
            // Find requests older than 24 hours that are still pending
            var cutoffTime = DateTime.UtcNow.AddHours(-24);
            
            var oldPendingRequests = await _dbContext.ServiceRequests
                .Where(r => r.Status == RequestStatus.Pending && r.CreatedAt < cutoffTime)
                .ToListAsync();

            if (!oldPendingRequests.Any())
            {
                return true;
            }

            // Mark all as expired
            foreach (var request in oldPendingRequests)
            {
                request.Status = RequestStatus.Expired;
                request.UpdatedAt = DateTime.UtcNow;
            }

            // Also expire pending request-provider relationships
            var requestIds = oldPendingRequests.Select(r => r.Id).ToList();
            var pendingRequestProviders = await _dbContext.RequestProviders
                .Where(rp => requestIds.Contains(rp.RequestId) && rp.Status == RequestProviderStatus.Pending)
                .ToListAsync();

            foreach (var rp in pendingRequestProviders)
            {
                rp.Status = RequestProviderStatus.Expired;
                rp.RespondedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}