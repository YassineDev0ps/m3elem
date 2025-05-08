using M3alam.Common.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.Interfaces
{
    public interface IRequestService
    {
        Task<RequestDetailsDto> GetDetailsAsync(Guid requestId);
        Task<IEnumerable<ProviderDto>> MatchProvidersAsync(MatchCriteriaDto criteria);
        Task<IEnumerable<ProviderDto>> SearchProvidersAsync(ProviderSearchDto criteria);
    }
}
