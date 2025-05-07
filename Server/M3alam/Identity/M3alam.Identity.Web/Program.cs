using M3alam.Identity.Application;
using M3alam.Identity.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using SchoolSaas.Infrastructure.Backoffice;

var builder = WebApplication.CreateBuilder(args);

// 1) Swagger (with Bearer support)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "M3alam API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Bearer authorization header: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 2) Database + Identity + OpenIddict
builder.Services.AddIdentityServerConfiguration(builder.Configuration);
builder.Services.AddIdentityInfrastructure (builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();

// 7) Authentication & Authorization (JWT Bearer against your OpenIddict server)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // 1) Don’t rely on Authority metadata
    options.RequireHttpsMetadata = false;

    // 2) Tell it explicitly how to validate your tokens:
    var jwtIssuer = builder.Configuration["Jwt:Issuer"];
    var jwtAudience = builder.Configuration["Jwt:Audience"];
    var jwtKeyHex = builder.Configuration["Jwt:Key"];

    // Convert your hex string to bytes
    var keyBytes = Enumerable.Range(0, jwtKeyHex.Length / 2)
        .Select(i => Convert.ToByte(jwtKeyHex.Substring(i * 2, 2), 16))
        .ToArray();

    var authority = builder.Configuration["OpenId:Authority"]?.TrimEnd('/');
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        // Accept both with and without slash
        ValidIssuers = new[] { authority, authority + "/" },

        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30),

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// 8) Seed roles, OpenIddict apps, and SuperUser on startup
using var scope = app.Services.CreateScope();
await Seeder.SeedRolesAsync(scope.ServiceProvider);
await Seeder.SeedOpenIddictAsync(scope.ServiceProvider);
await Seeder.SeedSuperUserAsync(scope.ServiceProvider);

// 9) Middleware pipeline
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "M3alam API v1"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
