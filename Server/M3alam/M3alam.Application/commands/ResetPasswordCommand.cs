public class ResetPasswordCommand : IRequest
{
    public ResetPasswordDto Dto { get; set; }

    public ResetPasswordCommand(ResetPasswordDto dto)
    {
        Dto = dto;
    }
}

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IIdentityService _service;

    public ResetPasswordCommandHandler(IIdentityService service)
    {
        _service = service;
    }

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        await _service.ResetPasswordAsync(request.Dto);
        return Unit.Value;
    }
}
