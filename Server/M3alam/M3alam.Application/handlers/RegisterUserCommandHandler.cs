// M3alam.Application/Handlers/RegisterUserCommandHandler.cs
using MediatR;
using M3alam.Infrastructure.Services; // Service layer
using M3alam.Common.DataTransferObjects; // DTOs
using System.Threading;
using System.Threading.Tasks;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
{
    private readonly IUserService _userService;

    public RegisterUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Delegate the logic to the service layer
        var result = await _userService.RegisterUserAsync(request.RegisterUserDto);
        return result;
    }
}
