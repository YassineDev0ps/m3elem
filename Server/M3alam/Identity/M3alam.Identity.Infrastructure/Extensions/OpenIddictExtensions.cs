using M3alam.Identity.Infrastructure.Context;
using M3alam.Identity.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace M3alam.Identity.Infrastructure.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityServerConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1) DbContext + OpenIddict Entity Framework
            services.AddDbContext<IdentityContext>(options =>
            {
                // Keep your exact connection string here
                options.UseSqlServer(
                    "Server=tcp:m3alam-db.database.windows.net,1433;Initial Catalog=M3alam_Database;Persist Security Info=False;User ID=Agile_Root;Password=M3alamAdmin;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
                    sql => sql.EnableRetryOnFailure()
                );
                options.UseOpenIddict(); // registers the OpenIddict stores
            });

            // 2) ASP.NET Core Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            // 3) OpenIddict
            services.AddOpenIddict()
                // Core: EF Core stores
                .AddCore(core =>
                {
                    core.UseEntityFrameworkCore()
                        .UseDbContext<IdentityContext>();
                })

                // Server: issue tokens
                .AddServer(options =>
                {
                    options.SetAuthorizationCodeLifetime(TimeSpan.FromHours(2));
                    options.SetAccessTokenLifetime(TimeSpan.FromHours(1));
                    options.SetIdentityTokenLifetime(TimeSpan.FromHours(6));
                    options.SetRefreshTokenLifetime(TimeSpan.FromDays(14));
                    options.SetUserCodeLifetime(TimeSpan.FromHours(4));

                    options.SetAuthorizationEndpointUris("/connect/authorize")
                           .SetDeviceEndpointUris("/connect/device")
                           .SetLogoutEndpointUris("/connect/logout")
                           .SetIntrospectionEndpointUris("/connect/introspect")
                           .SetTokenEndpointUris("/connect/token")
                           .SetUserinfoEndpointUris("/connect/userinfo")
                           .SetVerificationEndpointUris("/connect/verify");

                    options.AllowAuthorizationCodeFlow()
                           .AllowDeviceCodeFlow()
                           .AllowHybridFlow()
                           .AllowRefreshTokenFlow()
                           .AllowPasswordFlow();

                    options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles,
                        Scopes.OfflineAccess);

                    options.AddEphemeralEncryptionKey().AddEphemeralSigningKey();
                    options.DisableAccessTokenEncryption();

                    options.RequireProofKeyForCodeExchange();

                    options.UseAspNetCore()
                           .EnableStatusCodePagesIntegration()
                           .EnableAuthorizationEndpointPassthrough()
                           .EnableLogoutEndpointPassthrough()
                           .EnableTokenEndpointPassthrough()
                           .EnableUserinfoEndpointPassthrough()
                           .EnableVerificationEndpointPassthrough()
                           .DisableTransportSecurityRequirement();
                    options.AddEphemeralEncryptionKey()
        .AddEphemeralSigningKey();
                   
                    options.DisableScopeValidation();
                })

                // Validation: validate incoming tokens
                .AddValidation(validation =>
                {
                    validation.UseLocalServer();
                    validation.UseAspNetCore();  // from OpenIddict.Validation.AspNetCore
                });

            return services;
        }
    }
}
