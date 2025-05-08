public class GetThreadsQuery : IRequest<IEnumerable<ChatThreadDto>>
{
    public Guid UserId { get; }

    public GetThreadsQuery(Guid userId)
    {
        UserId = userId;
    }
}

public class GetThreadsQueryHandler : IRequestHandler<GetThreadsQuery, IEnumerable<ChatThreadDto>>
{
    private readonly IMessageService _service;

    public GetThreadsQueryHandler(IMessageService service)
    {
        _service = service;
    }

    public async Task<IEnumerable<ChatThreadDto>> Handle(GetThreadsQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetThreadsAsync(request.UserId);
    }
}
