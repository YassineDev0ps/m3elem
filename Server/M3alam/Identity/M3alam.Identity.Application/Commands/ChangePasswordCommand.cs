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
    public class ChangePasswordCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IIdentityService _service;
        public ChangePasswordCommandHandler(IIdentityService service) => _service = service;

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            await _service.ChangePasswordAsync(request.UserId, new ChangePasswordDto
            {
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword
            });
            return true;
        }
    }
}
