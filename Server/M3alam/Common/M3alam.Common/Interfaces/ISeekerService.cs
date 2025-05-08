using M3alam.Common.DataTransferObjects;
using M3alam.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.Interfaces
{
    public interface ISeekerService
    {
        Task<bool> CreateSeekerAccount(CreateSeekerAccountDto Data);
        Task<SeekerProfileDto> GetProfileAsync(Guid seekerId);
        Task UpdateProfileAsync(Guid seekerId, UpdateSeekerDto update);

        Task<IEnumerable<RequestDto>> GetRequestsAsync(Guid seekerId, RequestStatusEnum? status = null);
        Task<RequestDto> CreateRequestAsync(Guid seekerId, CreateRequestDto dto);
        Task CancelRequestAsync(Guid seekerId, Guid requestId);
        Task SelectProviderAsync(Guid seekerId, Guid requestId, Guid providerId);
        Task RateProviderAsync(Guid seekerId, Guid requestId, RatingDto rating);
    }
}
