
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
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public Guid UserId { get; set; }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IIdentityService _service;
        public GetUserByIdQueryHandler(IIdentityService service) => _service = service;

        public Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
            => _service.GetUserByIdAsync(request.UserId);
    }
}
