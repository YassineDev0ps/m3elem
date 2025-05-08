public class GetUserByIdQuery : IRequest<UserDto>
{
    public Guid UserId { get; set; }
}
