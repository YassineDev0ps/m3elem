public class UpdateProviderProfileCommand : IRequest<ProfileDto>
{
    public Guid ProviderId { get; }
    public UpdateProfileDto UpdateDto { get; }

    public UpdateProviderProfileCommand(Guid providerId, UpdateProfileDto updateDto)
    {
        ProviderId = providerId;
        UpdateDto = updateDto;
    }
}

public class UpdateProviderProfileCommandHandler : IRequestHandler<UpdateProviderProfileCommand, ProfileDto>
{
    private readonly IProviderService _service;

    public UpdateProviderProfileCommandHandler(IProviderService service)
    {
        _service = service;
    }

    public async Task<ProfileDto> Handle(UpdateProviderProfileCommand request, CancellationToken cancellationToken)
    {
        return await _service.UpdateProfileAsync(request.ProviderId, request.UpdateDto);
    }
}
