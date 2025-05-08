public class GetProviderProfileQuery : IRequest<ProfileDto>
{
    public Guid ProviderId { get; }

    public GetProviderProfileQuery(Guid providerId)
    {
        ProviderId = providerId;
    }
}

public class GetProviderProfileQueryHandler : IRequestHandler<GetProviderProfileQuery, ProfileDto>
{
    private readonly IProviderService _service;

    public GetProviderProfileQueryHandler(IProviderService service)
    {
        _service = service;
    }

    public async Task<ProfileDto> Handle(GetProviderProfileQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetProfileAsync(request.ProviderId);
    }
}
