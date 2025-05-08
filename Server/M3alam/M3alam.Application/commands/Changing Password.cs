
public class ChangePasswordCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public ChangePasswordDto Dto { get; set; }
}
