
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore;
using System.Linq;
using M3alam.Identity.Infrastructure.Identity;       // For LINQ extensions


[ApiController]
public class AuthorizationController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOpenIddictTokenManager _tokenManager;
    private readonly IOpenIddictScopeManager _scopeManager;

    public AuthorizationController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOpenIddictTokenManager tokenManager,
        IOpenIddictScopeManager scopeManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenManager = tokenManager;
        _scopeManager = scopeManager;
    }

    [HttpPost("~/connect/token"), Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
                      ?? throw new InvalidOperationException("Invalid OpenIddict request.");

        if (request.IsPasswordGrantType())
        {
            var user = await _userManager.FindByNameAsync(request.Username)
                       ?? await _userManager.FindByEmailAsync(request.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Invalid credentials."
                    }));
            }

            var principal = await CreateClaimsPrincipalAsync(user, request);
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsRefreshTokenGrantType())
        {
            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var user = await _userManager.FindByIdAsync(result.Principal.GetClaim(Claims.Subject));

            if (user == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "User no longer exists."
                    }));
            }

            // Create new principal to refresh claims
            var principal = await CreateClaimsPrincipalAsync(user, request);
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return BadRequest(new
        {
            error = Errors.UnsupportedGrantType,
            error_description = "The specified grant type is not supported."
        });
    }

    private async Task<ClaimsPrincipal> CreateClaimsPrincipalAsync(
     ApplicationUser user, OpenIddictRequest request)
    {
        // 1) Build the identity
        var identity = new ClaimsIdentity(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            Claims.Name, Claims.Role);

        identity.AddClaim(Claims.Subject, await _userManager.GetUserIdAsync(user));
        identity.AddClaim(Claims.Username, user.UserName ?? "");
        identity.AddClaim(Claims.Email, user.Email ?? "");

        // Add roles as individual claims (not space-separated)
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            identity.AddClaim(Claims.Role, role);
        }

        // 2) Create the principal
        var principal = new ClaimsPrincipal(identity);

        // 3) Scopes
        principal.SetScopes(request.GetScopes());

        // 4) Resources (audiences) — **ensure your API** is here
        //    Do NOT overwrite this later unless you have a real mapping.
        principal.SetResources("M3alam API");

        // 5) Destinations: only include what the client asked for
        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(GetDestinations(claim, principal));
        }

        return principal;
    }


    private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        switch (claim.Type)
        {
            case Claims.Name:
                yield return Destinations.AccessToken;
                if (principal.HasScope(Scopes.Profile))
                    yield return Destinations.IdentityToken;
                break;

            case Claims.Email:
                yield return Destinations.AccessToken;
                if (principal.HasScope(Scopes.Email))
                    yield return Destinations.IdentityToken;
                break;

            case Claims.Role:
                yield return Destinations.AccessToken;
                if (principal.HasScope(Scopes.Roles))
                    yield return Destinations.IdentityToken;
                break;

            // Exclude security stamp
            case "AspNet.Identity.SecurityStamp":
                yield break;

            default:
                yield return Destinations.AccessToken;
                break;
        }
    }

    [HttpPost("~/connect/logout")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Form["token"].ToString()?.Trim();

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Token is required.");
        }

        try
        {
            bool revoked = false;
            bool isJwt = token.Count(c => c == '.') is 2 or 4;

            if (isJwt)
            {
                var handler = new JwtSecurityTokenHandler();

                if (handler.CanReadToken(token))
                {
                    var jwt = handler.ReadJwtToken(token);
                    var tokenId = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                    if (!string.IsNullOrEmpty(tokenId))
                    {
                        // Revoke by token ID
                        var tokenEntity = await _tokenManager.FindByIdAsync(tokenId);
                        if (tokenEntity != null)
                        {
                            await _tokenManager.TryRevokeAsync(tokenEntity);
                            revoked = true;
                        }
                    }
                }
            }

            // Try reference token revocation if JWT revocation failed
            if (!revoked)
            {
                var tokenEntity = await _tokenManager.FindByReferenceIdAsync(token);
                if (tokenEntity != null)
                {
                    await _tokenManager.TryRevokeAsync(tokenEntity);
                    revoked = true;
                }
            }

            // Additional cleanup for refresh tokens
            if (revoked)
            {
                await _signInManager.SignOutAsync();

              
                return Ok(new { message = "Logout successful" });
            }

            return BadRequest("Token not found or already revoked");
        }
        catch (Exception ex)
        {
            return BadRequest($"Logout failed: {ex.Message}");
        }
    }

    //private async Task CleanupExpiredTokensAsync()
    //{
    //    await foreach (var token in _tokenManager.ListAsync())
    //    {
    //        if (token.ExpirationDate < DateTime.UtcNow)
    //        {
    //            await _tokenManager.TryRevokeAsync(token);
    //        }
    //    }
    //}
}