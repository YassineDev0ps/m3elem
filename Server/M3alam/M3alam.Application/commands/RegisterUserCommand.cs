public class RegisterUserCommand : IRequest<bool>
{
    public RegisterUserDto Dto { get; set; }

    public RegisterUserCommand(RegisterUserDto dto)
    {
        Dto = dto;
    }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
{
    private readonly IIdentityService _service;

    public RegisterUserCommandHandler(IIdentityService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _service.RegisterAsync(request.Dto);
    }
}
