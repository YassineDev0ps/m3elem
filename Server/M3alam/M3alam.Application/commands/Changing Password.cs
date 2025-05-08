public class ChangePasswordCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public ChangePasswordDto Dto { get; set; }

    public ChangePasswordCommand(Guid userId, ChangePasswordDto dto)
    {
        UserId = userId;
        Dto = dto;
    }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly IIdentityService _service;

    public ChangePasswordCommandHandler(IIdentityService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        return await _service.ChangePasswordAsync(request.UserId, request.Dto);
    }
}

