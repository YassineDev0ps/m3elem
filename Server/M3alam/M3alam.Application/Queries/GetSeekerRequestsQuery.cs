public class GetSeekerRequestsQuery : IRequest<IEnumerable<RequestDto>>
{
    public Guid SeekerId { get; }
    public RequestStatusEnum? Status { get; }

    public GetSeekerRequestsQuery(Guid seekerId, RequestStatusEnum? status = null)
    {
        SeekerId = seekerId;
        Status = status;
    }
}

public class GetSeekerRequestsQueryHandler : IRequestHandler<GetSeekerRequestsQuery, IEnumerable<RequestDto>>
{
    private readonly ISeekerService _service;

    public GetSeekerRequestsQueryHandler(ISeekerService service)
    {
        _service = service;
    }

    public async Task<IEnumerable<RequestDto>> Handle(GetSeekerRequestsQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetRequestsAsync(request.SeekerId, request.Status);
    }
}
