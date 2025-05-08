public class SendWorkerMessageCommand : IRequest<string>
{
    public string Message { get; }

    public SendWorkerMessageCommand(string message)
    {
        Message = message;
    }
}

public class SendWorkerMessageCommandHandler : IRequestHandler<SendWorkerMessageCommand, string>
{
    private readonly IWorkerService _service;

    public SendWorkerMessageCommandHandler(IWorkerService service)
    {
        _service = service;
    }

    public async Task<string> Handle(SendWorkerMessageCommand request, CancellationToken cancellationToken)
    {
        return await _service.SendAsync(request.Message);
    }
}
