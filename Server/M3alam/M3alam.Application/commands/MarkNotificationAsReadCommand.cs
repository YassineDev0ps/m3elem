public class MarkNotificationAsReadCommand : IRequest<bool>
{
    public Guid NotificationId { get; }

    public MarkNotificationAsReadCommand(Guid notificationId)
    {
        NotificationId = notificationId;
    }
}

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
{
    private readonly INotificationService _service;

    public MarkNotificationAsReadCommandHandler(INotificationService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        await _service.MarkAsReadAsync(request.NotificationId);
        return true;
    }
}
