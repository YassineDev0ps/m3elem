public class LogoutCommand : IRequest<bool>
{
    public string UserId { get; set; }

    public LogoutCommand(string userId)
    {
        UserId = userId;
    }
}

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        return await _authService.LogoutAsync(request.UserId);
    }
}
