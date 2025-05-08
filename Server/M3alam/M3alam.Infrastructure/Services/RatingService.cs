using Microsoft.EntityFrameworkCore;
using M3alem.Domain.Entities;
using M3alem.Infrastructure.Data;
using M3alem.Services.Rating.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace M3alem.Services.Rating.Services
{
    public class RatingService : IRatingService
    {
        private readonly ApplicationDbContext _dbContext;

        public RatingService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RatingDto> CreateRatingAsync(Guid seekerId, CreateRatingDto createDto)
        {
            // Validate the request exists and is completed
            var request = await _dbContext.ServiceRequests
                .Where(r => r.Id == createDto.RequestId && r.SeekerId == seekerId)
                .FirstOrDefaultAsync();

            if (request == null)
            {
                throw new ArgumentException("Invalid request ID or unauthorized");
            }

            if (request.Status != RequestStatus.Done)
            {
                throw new InvalidOperationException("Cannot rate a request that is not completed");
            }

            if (request.SelectedProviderId == null)
            {
                throw new InvalidOperationException("This request has no assigned provider to rate");
            }

            // Check if this request has already been rated
            var existingRating = await _dbContext.Ratings
                .AnyAsync(r => r.RequestId == createDto.RequestId);

            if (existingRating)
            {
                throw new InvalidOperationException("This request has already been rated");
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Create the rating
                var rating = new Domain.Entities.Rating
                {
                    Id = Guid.NewGuid(),
                    RequestId = createDto.RequestId,
                    ProviderId = request.SelectedProviderId.Value,
                    SeekerId = seekerId,
                    Stars = createDto.Stars,
                    Comment = createDto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.Ratings.Add(rating);
                await _dbContext.SaveChangesAsync();

                // Recalculate provider's average rating
                var providerId = request.SelectedProviderId.Value;
                var newAverage = await CalculateAverageRatingAsync(providerId);

                // Update provider's rating
                var provider = await _dbContext.ServiceProviders
                    .FirstOrDefaultAsync(p => p.Id == providerId);

                if (provider != null)
                {
                    provider.Rating = (float)newAverage;
                    await _dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                // Return the created rating
                return await GetRatingForRequestAsync(createDto.RequestId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<RatingDto> GetRatingForRequestAsync(Guid requestId)
        {
            var rating = await _dbContext.Ratings
                .Where(r => r.RequestId == requestId)
                .Include(r => r.Provider)
                .Include(r => r.Seeker)
                .Include(r => r.Request)
                .FirstOrDefaultAsync();

            if (rating == null)
            {
                return null;
            }

            return new RatingDto
            {
                Id = rating.Id,
                RequestId = rating.RequestId,
                ProviderId = rating.ProviderId,
                SeekerId = rating.SeekerId,
                Stars = rating.Stars,
                Comment = rating.Comment,
                CreatedAt = rating.CreatedAt,
                ProviderName = rating.Provider?.FullName,
                SeekerName = rating.Seeker?.FullName,
                ServiceTypeDescription = rating.Request?.ServiceType.ToString()
            };
        }

        public async Task<IEnumerable<RatingDto>> GetProviderRatingsAsync(Guid providerId)
        {
            var ratings = await _dbContext.Ratings
                .Where(r => r.ProviderId == providerId)
                .Include(r => r.Seeker)
                .Include(r => r.Request)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return ratings.Select(r => new RatingDto
            {
                Id = r.Id,
                RequestId = r.RequestId,
                ProviderId = r.ProviderId,
                SeekerId = r.SeekerId,
                Stars = r.Stars,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                SeekerName = r.Seeker?.FullName,
                ServiceTypeDescription = r.Request?.ServiceType.ToString()
            });
        }

        public async Task<double> CalculateAverageRatingAsync(Guid providerId)
        {
            var ratings = await _dbContext.Ratings
                .Where(r => r.ProviderId == providerId)
                .Select(r => r.Stars)
                .ToListAsync();

            if (!ratings.Any())
            {
                return 0;
            }

            return Math.Round(ratings.Average(), 1);
        }
    }
}