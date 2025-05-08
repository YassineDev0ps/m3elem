

public class UpdateUserCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public UpdateUserDto Dto { get; set; }
}
