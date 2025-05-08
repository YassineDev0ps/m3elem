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
        Task<bool> CreateProviderAccount(CreateProviderAccountDto Data);
        Task<ProfileDto> GetProfileAsync(Guid providerId);
        Task<ProfileDto> UpdateProfileAsync(Guid providerId, UpdateProfileDto updateDto);
        Task<DashboardDto> GetDashboardAsync(Guid providerId);
        Task<IEnumerable<RequestDto>> GetRequestsAsync(Guid providerId, RequestStatus? status = null);
        Task<bool> AcceptRequestAsync(Guid providerId, Guid requestId);
        Task<bool> DeclineRequestAsync(Guid providerId, Guid requestId);
        Task<bool> UpdateAvailabilityAsync(Guid providerId, bool availability);
    }
}
