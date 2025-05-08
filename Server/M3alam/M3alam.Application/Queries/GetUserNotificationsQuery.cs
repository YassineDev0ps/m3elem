public class GetUserNotificationsQuery : IRequest<List<NotificationDto>>
{
    public Guid UserId { get; }

    public GetUserNotificationsQuery(Guid userId)
    {
        UserId = userId;
    }
}

public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, List<NotificationDto>>
{
    private readonly INotificationService _service;

    public GetUserNotificationsQueryHandler(INotificationService service)
    {
        _service = service;
    }

    public async Task<List<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetUserNotificationsAsync(request.UserId);
    }
}
