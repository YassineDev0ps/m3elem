using M3alam.Common.DataTransferObjects;
using M3alam.Common.Interfaces;
using M3alam.Identity.Infrastructure.Context;
using M3alam.Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;  
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Abstractions.OpenIddictConstants.JsonWebTokenTypes;

namespace M3alam.Identity.Infrastructure.Service
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOpenIddictTokenManager _tokenManager;
        private readonly IdentityContext _dbContext;
        private readonly IConfiguration _configuration;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOpenIddictTokenManager tokenManager,
            IdentityContext dbContext,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenManager = tokenManager;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<bool> RegisterAsync(RegisterUserDto dto)
        {
            var user = new ApplicationUser { Email = dto.Email, UserName = dto.UserName, NormalizedEmail = dto.Email.Normalize() };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));


            return _userManager.AddToRoleAsync(user, dto.Role ?? "User").Result.Succeeded;
        }


        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email)
                       ?? throw new InvalidOperationException("Email not found");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // TODO: send token via email
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId.ToString())
                       ?? throw new InvalidOperationException("User not found");
            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            return new UserDto { Id = user.Id, Email = user.Email, Role = role };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return await Task.WhenAll(users.Select(async u =>
                new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = (await _userManager.GetRolesAsync(u)).FirstOrDefault()
                }));
        }

        public async Task<bool> UpdateUserAsync(Guid userId, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString())
                       ?? throw new InvalidOperationException("User not found");
            user.Email = dto.Email;
            user.UserName = dto.Email;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded && dto.Role != null)
            {
                var current = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, current);
                await _userManager.AddToRoleAsync(user, dto.Role);
            }
            return result.Succeeded;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString())
                       ?? throw new InvalidOperationException("User not found");
            return _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword).Result.Succeeded;
        }


    }
}
