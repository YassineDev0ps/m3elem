

public class RegisterUserCommand : IRequest<bool>
{
    public RegisterUserDto Dto { get; set; }
}
