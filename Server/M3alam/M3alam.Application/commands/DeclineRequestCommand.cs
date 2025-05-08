public class DeclineRequestCommand : IRequest<bool>
{
    public Guid ProviderId { get; }
    public Guid RequestId { get; }

    public DeclineRequestCommand(Guid providerId, Guid requestId)
    {
        ProviderId = providerId;
        RequestId = requestId;
    }
}

public class DeclineRequestCommandHandler : IRequestHandler<DeclineRequestCommand, bool>
{
    private readonly IProviderService _service;

    public DeclineRequestCommandHandler(IProviderService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(DeclineRequestCommand request, CancellationToken cancellationToken)
    {
        return await _service.DeclineRequestAsync(request.ProviderId, request.RequestId);
    }
}
