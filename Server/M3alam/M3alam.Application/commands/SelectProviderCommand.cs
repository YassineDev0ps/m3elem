public class SelectProviderCommand : IRequest
{
    public Guid SeekerId { get; }
    public Guid RequestId { get; }
    public Guid ProviderId { get; }

    public SelectProviderCommand(Guid seekerId, Guid requestId, Guid providerId)
    {
        SeekerId = seekerId;
        RequestId = requestId;
        ProviderId = providerId;
    }
}

public class SelectProviderCommandHandler : IRequestHandler<SelectProviderCommand>
{
    private readonly ISeekerService _service;

    public SelectProviderCommandHandler(ISeekerService service)
    {
        _service = service;
    }

    public async Task Handle(SelectProviderCommand request, CancellationToken cancellationToken)
    {
        await _service.SelectProviderAsync(request.SeekerId, request.RequestId, request.ProviderId);
    }
}
