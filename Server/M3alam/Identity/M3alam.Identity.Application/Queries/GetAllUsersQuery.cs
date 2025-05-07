
using M3alam.Common.DataTransferObjects;
using M3alam.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Identity.Application.Queries
{
    public class GetAllUsersQuery : IRequest<List<UserDto>>
    {
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
    {
        private readonly IIdentityService _service;
        public GetAllUsersQueryHandler(IIdentityService service) => _service = service;

        public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
            => (await _service.GetAllUsersAsync()).ToList();
    }
}
