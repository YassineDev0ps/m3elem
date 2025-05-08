public class ExpireOldPendingRequestsCommand : IRequest<bool>
{
}

public class ExpireOldPendingRequestsCommandHandler : IRequestHandler<ExpireOldPendingRequestsCommand, bool>
{
    private readonly IRequestService _service;

    public ExpireOldPendingRequestsCommandHandler(IRequestService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(ExpireOldPendingRequestsCommand request, CancellationToken cancellationToken)
    {
        return await _service.ExpireOldPendingRequestsAsync();
    }
}
