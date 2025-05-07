
using M3alam.Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace M3alam.Identity.Infrastructure.Extensions
{
    public class Seeder
    {
        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            var roles = new[] { "SuperUser", "Admin", "Provider", "Recipient" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = role,
                        NormalizedName = role.Normalize(),
                        
                    });
                }
            }
        }

        public static async Task SeedOpenIddictAsync(IServiceProvider services)
        {
            var appManager = services.GetRequiredService<IOpenIddictApplicationManager>();

            if (await appManager.FindByClientIdAsync("web-client") is null)
            {
                await appManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "web-client",
                    ConsentType = ConsentTypes.Implicit,
                    DisplayName = "Web Frontend",
                    ClientType = ClientTypes.Public,
                    PostLogoutRedirectUris = { new Uri("https://localhost:3000/logout") },
                    RedirectUris = { new Uri("https://localhost:3000/auth/callback") },
                    Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.Password,
                Permissions.GrantTypes.RefreshToken,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Scopes.OfflineAccess,
                Scopes.OpenId
            }
                });
            }
        }
        public static async Task SeedSuperUserAsync(IServiceProvider services)
        {
            var userMgr = services.GetRequiredService<UserManager<ApplicationUser>>();
            var cfg = services.GetRequiredService<IConfiguration>();
            var email = cfg["DefaultSuperUser:Email"];
            var pwd = cfg["DefaultSuperUser:Password"];

            if (await userMgr.FindByEmailAsync(email) is null)
            {
                var user = new ApplicationUser { Email = email, UserName = email };
                var res = await userMgr.CreateAsync(user, pwd);
                if (res.Succeeded)
                    await userMgr.AddToRoleAsync(user, "SuperUser");
            }
        }


    }
}
