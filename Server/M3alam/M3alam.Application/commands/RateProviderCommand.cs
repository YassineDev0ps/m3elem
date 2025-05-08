public class RateProviderCommand : IRequest
{
    public Guid SeekerId { get; }
    public Guid RequestId { get; }
    public RatingDto Rating { get; }

    public RateProviderCommand(Guid seekerId, Guid requestId, RatingDto rating)
    {
        SeekerId = seekerId;
        RequestId = requestId;
        Rating = rating;
    }
}

public class RateProviderCommandHandler : IRequestHandler<RateProviderCommand>
{
    private readonly ISeekerService _service;

    public RateProviderCommandHandler(ISeekerService service)
    {
        _service = service;
    }

    public async Task Handle(RateProviderCommand request, CancellationToken cancellationToken)
    {
        await _service.RateProviderAsync(request.SeekerId, request.RequestId, request.Rating);
    }
}
