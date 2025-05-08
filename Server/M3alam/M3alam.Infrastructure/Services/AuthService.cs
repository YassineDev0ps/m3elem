using M3alem.Domain.Entities;
using M3alem.Infrastructure.Data;
using M3alem.Services.Identity.DTOs;
using M3alem.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace M3alem.Services.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterSeekerAsync(RegisterSeekerDto registerDto)
        {
            // Check if user exists
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User already exists"
                };
            }

            // Create the Identity user
            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                FullName = registerDto.FullName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Add to Seeker role
            await _userManager.AddToRoleAsync(user, "Seeker");

            // Create the domain seeker entity
            var seeker = new ServiceSeeker
            {
                Id = user.Id,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                Latitude = registerDto.Latitude,
                Longitude = registerDto.Longitude,
                Role = UserRole.Seeker
            };

            _dbContext.ServiceSeekers.Add(seeker);
            await _dbContext.SaveChangesAsync();

            // Generate token
            var token = await GenerateJwtTokenAsync(user, "Seeker");

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                Role = "Seeker",
                UserId = user.Id.ToString(),
                Message = "User registered successfully"
            };
        }

        public async Task<AuthResponseDto> RegisterProviderAsync(RegisterProviderDto registerDto)
        {
            // Check if user exists
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User already exists"
                };
            }

            // Create the Identity user
            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                FullName = registerDto.FullName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // Add to Provider role
            await _userManager.AddToRoleAsync(user, "Provider");

            // Create the domain provider entity
            var provider = new ServiceProvider
            {
                Id = user.Id,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                Phone = registerDto.Phone,
                Skills = registerDto.Skills,
                ExperienceYears = registerDto.ExperienceYears,
                Availability = registerDto.Availability,
                Latitude = registerDto.Latitude,
                Longitude = registerDto.Longitude,
                Bio = registerDto.Bio,
                Role = UserRole.Provider
            };

            _dbContext.ServiceProviders.Add(provider);
            await _dbContext.SaveChangesAsync();

            // Generate token
            var token = await GenerateJwtTokenAsync(user, "Provider");

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                Role = "Provider",
                UserId = user.Id.ToString(),
                Message = "User registered successfully"
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            // Get user's role
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            // Generate token
            var token = await GenerateJwtTokenAsync(user, role);

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                Role = role,
                UserId = user.Id.ToString(),
                Message = "Login successful"
            };
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            // In a JWT-based auth, we typically don't need server-side logout
            // But we could implement token blacklisting here if needed
            return true;
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);
            var tokenExpiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}