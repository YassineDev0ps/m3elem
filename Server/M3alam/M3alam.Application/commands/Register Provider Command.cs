public class RegisterProviderCommand : IRequest<AuthResponseDto>
{
    public RegisterProviderDto ProviderDto { get; set; }

    public RegisterProviderCommand(RegisterProviderDto providerDto)
    {
        ProviderDto = providerDto;
    }
}

public class RegisterProviderCommandHandler : IRequestHandler<RegisterProviderCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public RegisterProviderCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(RegisterProviderCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RegisterProviderAsync(request.ProviderDto);
    }
}