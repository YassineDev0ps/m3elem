public class GetSeekerProfileQuery : IRequest<SeekerProfileDto>
{
    public Guid SeekerId { get; }

    public GetSeekerProfileQuery(Guid seekerId)
    {
        SeekerId = seekerId;
    }
}

public class GetSeekerProfileQueryHandler : IRequestHandler<GetSeekerProfileQuery, SeekerProfileDto>
{
    private readonly ISeekerService _service;

    public GetSeekerProfileQueryHandler(ISeekerService service)
    {
        _service = service;
    }

    public async Task<SeekerProfileDto> Handle(GetSeekerProfileQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetProfileAsync(request.SeekerId);
    }
}
