public class GetProviderDashboardQuery : IRequest<DashboardDto>
{
    public Guid ProviderId { get; }

    public GetProviderDashboardQuery(Guid providerId)
    {
        ProviderId = providerId;
    }
}

public class GetProviderDashboardQueryHandler : IRequestHandler<GetProviderDashboardQuery, DashboardDto>
{
    private readonly IProviderService _service;

    public GetProviderDashboardQueryHandler(IProviderService service)
    {
        _service = service;
    }

    public async Task<DashboardDto> Handle(GetProviderDashboardQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetDashboardAsync(request.ProviderId);
    }
}
