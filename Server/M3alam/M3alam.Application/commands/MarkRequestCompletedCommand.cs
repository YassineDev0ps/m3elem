public class MarkRequestCompletedCommand : IRequest<bool>
{
    public Guid RequestId { get; }

    public MarkRequestCompletedCommand(Guid requestId)
    {
        RequestId = requestId;
    }
}

public class MarkRequestCompletedCommandHandler : IRequestHandler<MarkRequestCompletedCommand, bool>
{
    private readonly IRequestService _service;

    public MarkRequestCompletedCommandHandler(IRequestService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(MarkRequestCompletedCommand request, CancellationToken cancellationToken)
    {
        return await _service.MarkRequestCompletedAsync(request.RequestId);
    }
}
