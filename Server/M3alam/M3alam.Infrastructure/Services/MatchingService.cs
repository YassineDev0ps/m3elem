using Microsoft.EntityFrameworkCore;
using M3alem.Domain.Entities;
using M3alem.Infrastructure.Data;
using M3alem.Services.Matching.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace M3alem.Services.Matching.Services
{
    public class MatchingService : IMatchingService
    {
        private readonly ApplicationDbContext _dbContext;

        public MatchingService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ProviderMatchDto>> FindMatchingProvidersAsync(MatchRequestDto matchRequest)
        {
            // Get available providers with the requested service type
            var serviceType = matchRequest.ServiceType.ToString();
            
            var providers = await _dbContext.ServiceProviders
                .Where(p => p.Availability == true && p.Skills.Contains(serviceType))
                .ToListAsync();

            // Calculate distance for each provider and filter by max distance
            var matchedProviders = providers
                .Select(p => new
                {
                    Provider = p,
                    Distance = CalculateDistance(
                        matchRequest.Latitude, matchRequest.Longitude,
                        p.Latitude, p.Longitude
                    )
                })
                .Where(x => x.Distance <= matchRequest.MaxDistanceKm)
                .OrderBy(x => x.Distance)
                .ThenByDescending(x => x.Provider.Rating)
                .Take(matchRequest.MaxResults)
                .Select(x => new ProviderMatchDto
                {
                    Id = x.Provider.Id,
                    FullName = x.Provider.FullName,
                    Phone = x.Provider.Phone,
                    Skills = x.Provider.Skills,
                    ExperienceYears = x.Provider.ExperienceYears,
                    Latitude = x.Provider.Latitude,
                    Longitude = x.Provider.Longitude,
                    Bio = x.Provider.Bio,
                    Rating = x.Provider.Rating,
                    TotalJobs = x.Provider.TotalJobs,
                    DistanceKm = Math.Round(x.Distance, 1)
                });

            return matchedProviders;
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
    }
}