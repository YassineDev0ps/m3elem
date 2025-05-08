public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>> { }

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IIdentityService _service;

    public GetAllUsersQueryHandler(IIdentityService service)
    {
        _service = service;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await _service.GetAllUsersAsync();
    }
}
