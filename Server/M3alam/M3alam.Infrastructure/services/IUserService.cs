// M3alam.Infrastructure/Services/IUserService.cs
using M3alam.Common.DataTransferObjects; // DTOs
using System.Threading.Tasks;

public interface IUserService
{
    Task<bool> RegisterUserAsync(RegisterUserDto registerUserDto); // Register a new user
    Task<UserDto> GetUserByIdAsync(Guid userId); // Get user details by ID
}
