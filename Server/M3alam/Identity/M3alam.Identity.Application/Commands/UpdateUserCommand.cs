
using M3alam.Common.DataTransferObjects;
using M3alam.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Identity.Application.Commands
{
    
    public class UpdateUserCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IIdentityService _service;
        public UpdateUserCommandHandler(IIdentityService service) => _service = service;

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _service.UpdateUserAsync(request.UserId, new UpdateUserDto
            {
                Email = request.Email,
                Role = request.Role
            });
            return result;
        }
    }

}
