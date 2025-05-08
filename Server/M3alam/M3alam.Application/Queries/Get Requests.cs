public class GetProviderRequestsQuery : IRequest<IEnumerable<RequestDto>>
{
    public Guid ProviderId { get; }
    public RequestStatus? Status { get; }

    public GetProviderRequestsQuery(Guid providerId, RequestStatus? status = null)
    {
        ProviderId = providerId;
        Status = status;
    }
}

public class GetProviderRequestsQueryHandler : IRequestHandler<GetProviderRequestsQuery, IEnumerable<RequestDto>>
{
    private readonly IProviderService _service;

    public GetProviderRequestsQueryHandler(IProviderService service)
    {
        _service = service;
    }

    public async Task<IEnumerable<RequestDto>> Handle(GetProviderRequestsQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetRequestsAsync(request.ProviderId, request.Status);
    }
}

