public class UpdateUserCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public UpdateUserDto Dto { get; set; }

    public UpdateUserCommand(Guid userId, UpdateUserDto dto)
    {
        UserId = userId;
        Dto = dto;
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IIdentityService _service;

    public UpdateUserCommandHandler(IIdentityService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await _service.UpdateUserAsync(request.UserId, request.Dto);
    }
}
