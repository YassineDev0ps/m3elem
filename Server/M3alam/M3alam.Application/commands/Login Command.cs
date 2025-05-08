public class LoginCommand : IRequest<AuthResponseDto>
{
    public LoginDto LoginDto { get; set; }

    public LoginCommand(LoginDto loginDto)
    {
        LoginDto = loginDto;
    }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authService.LoginAsync(request.LoginDto);
    }
}
