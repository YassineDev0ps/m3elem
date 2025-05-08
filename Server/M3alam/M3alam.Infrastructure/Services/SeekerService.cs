using Microsoft.EntityFrameworkCore;
using M3alem.Domain.Entities;
using M3alem.Infrastructure.Data;
using M3alem.Services.Seeker.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace M3alem.Services.Seeker.Services
{
    public class SeekerService : ISeekerService
    {
        private readonly ApplicationDbContext _dbContext;

        public SeekerService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProfileDto> GetProfileAsync(Guid seekerId)
        {
            var seeker = await _dbContext.ServiceSeekers
                .Where(s => s.Id == seekerId)
                .FirstOrDefaultAsync();

            if (seeker == null)
            {
                return null;
            }

            return new ProfileDto
            {
                Id = seeker.Id,
                Email = seeker.Email,
                FullName = seeker.FullName,
                Phone = seeker.Phone,
                Address = seeker.Address,
                Latitude = seeker.Latitude,
                Longitude = seeker.Longitude
            };
        }

        public async Task<ProfileDto> UpdateProfileAsync(Guid seekerId, UpdateProfileDto updateDto)
        {
            var seeker = await _dbContext.ServiceSeekers
                .Where(s => s.Id == seekerId)
                .FirstOrDefaultAsync();

            if (seeker == null)
            {
                return null;
            }

            // Update properties
            seeker.FullName = updateDto.FullName;
            seeker.Phone = updateDto.Phone;
            seeker.Address = updateDto.Address;
            seeker.Latitude = updateDto.Latitude;
            seeker.Longitude = updateDto.Longitude;

            await _dbContext.SaveChangesAsync();

            return new ProfileDto
            {
                Id = seeker.Id,
                Email = seeker.Email,
                FullName = seeker.FullName,
                Phone = seeker.Phone,
                Address = seeker.Address,
                Latitude = seeker.Latitude,
                Longitude = seeker.Longitude
            };
        }

        public async Task<IEnumerable<RequestSummaryDto>> GetRequestsAsync(Guid seekerId, RequestStatus? status = null)
        {
            var query = _dbContext.ServiceRequests
                .Where(r => r.SeekerId == seekerId)
                .Include(r => r.SelectedProvider)
                .Include(r => r.Rating)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            var requests = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();

            return requests.Select(r => new RequestSummaryDto
            {
                Id = r.Id,
                ServiceType = r.ServiceType,
                Description = r.Description,
                Status = r.Status,
                ScheduledDateTime = r.ScheduledDateTime,
                CreatedAt = r.CreatedAt,
                ProviderId = r.SelectedProviderId,
                ProviderName = r.SelectedProvider?.FullName,
                ProviderPhone = r.SelectedProvider?.Phone,
                ProviderRating = r.SelectedProvider?.Rating,
                IsRated = r.Rating != null
            });
        }

        public async Task<bool> CancelRequestAsync(Guid seekerId, Guid requestId)
        {
            var request = await _dbContext.ServiceRequests
                .Where(r => r.Id == requestId && r.SeekerId == seekerId)
                .FirstOrDefaultAsync();

            if (request == null)
            {
                return false;
            }

            // Only allow cancellation for Pending requests or those InProgress (with restrictions)
            if (request.Status == RequestStatus.Pending)
            {
                request.Status = RequestStatus.Cancelled;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else if (request.Status == RequestStatus.InProgress)
            {
                // Add business logic for cancellation after acceptance if needed
                // For example, might require a reason or penalty
                request.Status = RequestStatus.Cancelled;
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}