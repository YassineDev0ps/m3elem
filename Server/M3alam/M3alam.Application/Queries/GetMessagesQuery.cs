public class GetMessagesQuery : IRequest<IEnumerable<MessageDto>>
{
    public Guid RequestId { get; }

    public GetMessagesQuery(Guid requestId)
    {
        RequestId = requestId;
    }
}

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, IEnumerable<MessageDto>>
{
    private readonly IMessageService _service;

    public GetMessagesQueryHandler(IMessageService service)
    {
        _service = service;
    }

    public async Task<IEnumerable<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetMessagesAsync(request.RequestId);
    }
}
