using MediatR;
using M3alem.Services.Identity.DTOs;

public class RegisterSeekerCommand : IRequest<AuthResponseDto>
{
    public RegisterSeekerDto SeekerDto { get; set; }

    public RegisterSeekerCommand(RegisterSeekerDto seekerDto)
    {
        SeekerDto = seekerDto;
    }
}

public class RegisterSeekerCommandHandler : IRequestHandler<RegisterSeekerCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public RegisterSeekerCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(RegisterSeekerCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RegisterSeekerAsync(request.SeekerDto);
    }
}