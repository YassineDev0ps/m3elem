public class GetRatingForRequestQuery : IRequest<RatingDto>
{
    public Guid RequestId { get; }

    public GetRatingForRequestQuery(Guid requestId)
    {
        RequestId = requestId;
    }
}

public class GetRatingForRequestQueryHandler : IRequestHandler<GetRatingForRequestQuery, RatingDto>
{
    private readonly IRatingService _service;

    public GetRatingForRequestQueryHandler(IRatingService service)
    {
        _service = service;
    }

    public async Task<RatingDto> Handle(GetRatingForRequestQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetRatingForRequestAsync(request.RequestId);
    }
}
