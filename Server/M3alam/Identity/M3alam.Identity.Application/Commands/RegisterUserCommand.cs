
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M3alam.Common.DataTransferObjects;
using M3alam.Common.Interfaces;
using MediatR;


namespace M3alam.Identity.Application.Commands
{
    public class RegisterUserCommand : IRequest<bool>
    {
        public RegisterUserDto Data { get; set; }
    }
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly IIdentityService _service;
        public RegisterUserCommandHandler(IIdentityService service) => _service = service;

        public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {


            return await _service.RegisterAsync(request.Data);
        }
    }
}
