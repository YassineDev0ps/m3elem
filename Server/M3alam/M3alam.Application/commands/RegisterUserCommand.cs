// M3alam.Application/Commands/RegisterUserCommand.cs
using MediatR;
using M3alam.Common.DataTransferObjects;

public class RegisterUserCommand : IRequest<bool>
{
    public RegisterUserDto RegisterUserDto { get; set; }

    public RegisterUserCommand(RegisterUserDto registerUserDto)
    {
        RegisterUserDto = registerUserDto;
    }
}
