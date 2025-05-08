public class CreateNotificationCommand : IRequest<bool>
{
    public Guid UserId { get; }
    public string Message { get; }

    public CreateNotificationCommand(Guid userId, string message)
    {
        UserId = userId;
        Message = message;
    }
}

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, bool>
{
    private readonly INotificationService _service;

    public CreateNotificationCommandHandler(INotificationService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        await _service.CreateAsync(request.UserId, request.Message);
        return true;
    }
}
