
using M3alam.Common.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M3alam.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<bool> RegisterAsync(RegisterUserDto dto);
       
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordDto dto);
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(Guid userId, UpdateUserDto dto);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    }
}
