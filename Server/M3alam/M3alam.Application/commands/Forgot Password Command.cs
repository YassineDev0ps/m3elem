public class ForgotPasswordCommand : IRequest
{
    public string Email { get; set; }

    public ForgotPasswordCommand(string email)
    {
        Email = email;
    }
}

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IIdentityService _service;

    public ForgotPasswordCommandHandler(IIdentityService service)
    {
        _service = service;
    }

    public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        await _service.ForgotPasswordAsync(request.Email);
        return Unit.Value;
    }
}
