public class CreateSeekerRequestCommand : IRequest<RequestDto>
{
    public Guid SeekerId { get; }
    public CreateRequestDto Dto { get; }

    public CreateSeekerRequestCommand(Guid seekerId, CreateRequestDto dto)
    {
        SeekerId = seekerId;
        Dto = dto;
    }
}

public class CreateSeekerRequestCommandHandler : IRequestHandler<CreateSeekerRequestCommand, RequestDto>
{
    private readonly ISeekerService _service;

    public CreateSeekerRequestCommandHandler(ISeekerService service)
    {
        _service = service;
    }

    public async Task<RequestDto> Handle(CreateSeekerRequestCommand request, CancellationToken cancellationToken)
    {
        return await _service.CreateRequestAsync(request.SeekerId, request.Dto);
    }
}
