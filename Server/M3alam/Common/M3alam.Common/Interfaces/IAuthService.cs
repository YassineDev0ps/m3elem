using M3alem.Services.Identity.DTOs;
using System.Threading.Tasks;

namespace M3alem.Services.Identity.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterSeekerAsync(RegisterSeekerDto registerDto);
        Task<AuthResponseDto> RegisterProviderAsync(RegisterProviderDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<bool> LogoutAsync(string userId);
    }
}