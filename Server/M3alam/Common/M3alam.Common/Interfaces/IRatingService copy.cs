using M3alem.Services.Rating.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace M3alem.Services.Rating.Services
{
    public interface IRatingService
    {
        Task<RatingDto> CreateRatingAsync(Guid seekerId, CreateRatingDto createDto);
        Task<RatingDto> GetRatingForRequestAsync(Guid requestId);
        Task<IEnumerable<RatingDto>> GetProviderRatingsAsync(Guid providerId);
        Task<double> CalculateAverageRatingAsync(Guid providerId);
    }
}