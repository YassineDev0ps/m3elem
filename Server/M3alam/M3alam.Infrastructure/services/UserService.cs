// M3alam.Infrastructure/Services/UserService.cs
using M3alam.Domain.Entities;
using M3alam.Common.DataTransferObjects;
using M3alam.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        // Hash the password before saving
        var hashedPassword = HashPassword(registerUserDto.Password);

        var user = new User
        {
            FullName = registerUserDto.FullName,
            Email = registerUserDto.Email,
            Password = hashedPassword // Store the hashed password
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email
        };
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
