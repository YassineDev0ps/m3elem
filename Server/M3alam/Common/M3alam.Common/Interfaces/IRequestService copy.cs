using M3alem.Services.Request.DTOs;
using System;
using System.Threading.Tasks;

namespace M3alem.Services.Request.Services
{
    public interface IRequestService
    {
        Task<Guid> CreateRequestAsync(Guid seekerId, CreateRequestDto createDto);
        Task<RequestDetailDto> GetRequestDetailAsync(Guid requestId);
        Task<bool> MarkRequestCompletedAsync(Guid requestId);
        Task<bool> ExpireOldPendingRequestsAsync();
    }
}