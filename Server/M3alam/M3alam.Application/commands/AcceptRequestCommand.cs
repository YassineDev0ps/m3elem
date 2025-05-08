public class AcceptRequestCommand : IRequest<bool>
{
    public Guid ProviderId { get; }
    public Guid RequestId { get; }

    public AcceptRequestCommand(Guid providerId, Guid requestId)
    {
        ProviderId = providerId;
        RequestId = requestId;
    }
}

public class AcceptRequestCommandHandler : IRequestHandler<AcceptRequestCommand, bool>
{
    private readonly IProviderService _service;

    public AcceptRequestCommandHandler(IProviderService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(AcceptRequestCommand request, CancellationToken cancellationToken)
    {
        return await _service.AcceptRequestAsync(request.ProviderId, request.RequestId);
    }
}
