public class CalculateAverageRatingQuery : IRequest<double>
{
    public Guid ProviderId { get; }

    public CalculateAverageRatingQuery(Guid providerId)
    {
        ProviderId = providerId;
    }
}

public class CalculateAverageRatingQueryHandler : IRequestHandler<CalculateAverageRatingQuery, double>
{
    private readonly IRatingService _service;

    public CalculateAverageRatingQueryHandler(IRatingService service)
    {
        _service = service;
    }

    public async Task<double> Handle(CalculateAverageRatingQuery request, CancellationToken cancellationToken)
    {
        return await _service.CalculateAverageRatingAsync(request.ProviderId);
    }
}
