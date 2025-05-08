public class GetRequestDetailQuery : IRequest<RequestDetailDto>
{
    public Guid RequestId { get; }

    public GetRequestDetailQuery(Guid requestId)
    {
        RequestId = requestId;
    }
}

public class GetRequestDetailQueryHandler : IRequestHandler<GetRequestDetailQuery, RequestDetailDto>
{
    private readonly IRequestService _service;

    public GetRequestDetailQueryHandler(IRequestService service)
    {
        _service = service;
    }

    public async Task<RequestDetailDto> Handle(GetRequestDetailQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetRequestDetailAsync(request.RequestId);
    }
}
