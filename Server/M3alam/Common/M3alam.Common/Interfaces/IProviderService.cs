using M3alam.Common.DataTransferObjects;
using M3alam.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.Interfaces
{
    public interface IProviderService
    {
        Task<ProviderProfileDto> GetProfileAsync(Guid providerId);
        Task UpdateProfileAsync(Guid providerId, UpdateProviderDto update);

        Task<DashboardDto> GetDashboardAsync(Guid providerId);
        Task<IEnumerable<RequestDto>> GetRequestsAsync(Guid providerId, RequestStatusEnum? status = null);

        Task AcceptRequestAsync(Guid providerId, Guid requestId);
        Task DeclineRequestAsync(Guid providerId, Guid requestId);
        Task StartRequestAsync(Guid providerId, Guid requestId);
        Task CompleteRequestAsync(Guid providerId, Guid requestId);
    }
}
