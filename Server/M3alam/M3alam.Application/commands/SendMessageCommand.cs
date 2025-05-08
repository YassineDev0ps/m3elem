public class SendMessageCommand : IRequest<bool>
{
    public Guid RequestId { get; }
    public Guid SenderId { get; }
    public CreateMessageDto Message { get; }

    public SendMessageCommand(Guid requestId, Guid senderId, CreateMessageDto message)
    {
        RequestId = requestId;
        SenderId = senderId;
        Message = message;
    }
}

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, bool>
{
    private readonly IMessageService _service;

    public SendMessageCommandHandler(IMessageService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        await _service.SendMessageAsync(request.RequestId, request.SenderId, request.Message);
        return true;
    }
}
