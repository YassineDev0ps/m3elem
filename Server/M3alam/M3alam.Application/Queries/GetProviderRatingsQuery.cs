public class GetProviderRatingsQuery : IRequest<IEnumerable<RatingDto>>
{
    public Guid ProviderId { get; }

    public GetProviderRatingsQuery(Guid providerId)
    {
        ProviderId = providerId;
    }
}

public class GetProviderRatingsQueryHandler : IRequestHandler<GetProviderRatingsQuery, IEnumerable<RatingDto>>
{
    private readonly IRatingService _service;

    public GetProviderRatingsQueryHandler(IRatingService service)
    {
        _service = service;
    }

    public async Task<IEnumerable<RatingDto>> Handle(GetProviderRatingsQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetProviderRatingsAsync(request.ProviderId);
    }
}
