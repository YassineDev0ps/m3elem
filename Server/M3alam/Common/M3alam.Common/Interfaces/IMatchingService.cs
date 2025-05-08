using M3alem.Services.Matching.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace M3alem.Services.Matching.Services
{
    public interface IMatchingService
    {
        Task<IEnumerable<ProviderMatchDto>> FindMatchingProvidersAsync(MatchRequestDto matchRequest);
    }
}