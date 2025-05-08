public class GetUserByIdQuery : IRequest<UserDto>
{
    public Guid UserId { get; set; }

    public GetUserByIdQuery(Guid userId)
    {
        UserId = userId;
    }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IIdentityService _service;

    public GetUserByIdQueryHandler(IIdentityService service)
    {
        _service = service;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetUserByIdAsync(request.UserId);
    }
}
