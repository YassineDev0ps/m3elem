
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
    public class ResetPasswordCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IIdentityService _service;
        public ResetPasswordCommandHandler(IIdentityService service) => _service = service;

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            await _service.ResetPasswordAsync(new ResetPasswordDto
            {
                UserId = request.UserId,
                Token = request.Token,
                NewPassword = request.NewPassword
            });
            return true;
        }
    }
}
