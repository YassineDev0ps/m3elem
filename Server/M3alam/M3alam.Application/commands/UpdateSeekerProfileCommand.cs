public class UpdateSeekerProfileCommand : IRequest
{
    public Guid SeekerId { get; }
    public UpdateSeekerDto UpdateDto { get; }

    public UpdateSeekerProfileCommand(Guid seekerId, UpdateSeekerDto updateDto)
    {
        SeekerId = seekerId;
        UpdateDto = updateDto;
    }
}

public class UpdateSeekerProfileCommandHandler : IRequestHandler<UpdateSeekerProfileCommand>
{
    private readonly ISeekerService _service;

    public UpdateSeekerProfileCommandHandler(ISeekerService service)
    {
        _service = service;
    }

    public async Task Handle(UpdateSeekerProfileCommand request, CancellationToken cancellationToken)
    {
        await _service.UpdateProfileAsync(request.SeekerId, request.UpdateDto);
    }
}
