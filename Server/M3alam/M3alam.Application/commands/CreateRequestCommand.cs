public class CreateRequestCommand : IRequest<Guid>
{
    public Guid SeekerId { get; }
    public CreateRequestDto CreateDto { get; }

    public CreateRequestCommand(Guid seekerId, CreateRequestDto createDto)
    {
        SeekerId = seekerId;
        CreateDto = createDto;
    }
}

public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, Guid>
{
    private readonly IRequestService _service;

    public CreateRequestCommandHandler(IRequestService service)
    {
        _service = service;
    }

    public async Task<Guid> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        return await _service.CreateRequestAsync(request.SeekerId, request.CreateDto);
    }
}
