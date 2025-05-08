public class UpdateProviderAvailabilityCommand : IRequest<bool>
{
    public Guid ProviderId { get; }
    public bool Availability { get; }

    public UpdateProviderAvailabilityCommand(Guid providerId, bool availability)
    {
        ProviderId = providerId;
        Availability = availability;
    }
}

public class UpdateProviderAvailabilityCommandHandler : IRequestHandler<UpdateProviderAvailabilityCommand, bool>
{
    private readonly IProviderService _service;

    public UpdateProviderAvailabilityCommandHandler(IProviderService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(UpdateProviderAvailabilityCommand request, CancellationToken cancellationToken)
    {
        return await _service.UpdateAvailabilityAsync(request.ProviderId, request.Availability);
    }
}
