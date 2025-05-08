public class CancelRequestCommand : IRequest
{
    public Guid SeekerId { get; }
    public Guid RequestId { get; }

    public CancelRequestCommand(Guid seekerId, Guid requestId)
    {
        SeekerId = seekerId;
        RequestId = requestId;
    }
}

public class CancelRequestCommandHandler : IRequestHandler<CancelRequestCommand>
{
    private readonly ISeekerService _service;

    public CancelRequestCommandHandler(ISeekerService service)
    {
        _service = service;
    }

    public async Task Handle(CancelRequestCommand request, CancellationToken cancellationToken)
    {
        await _service.CancelRequestAsync(request.SeekerId, request.RequestId);
    }
}
