using Relay.IdentityServer.Infrastructure.Data.Entities;

namespace Relay.IdentityServer.Infrastructure.Factories;

public class ApplicationUserClaimsPrincipalFactory(
    UserManager<User> userManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<User>(userManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        var roles = await UserManager.GetRolesAsync(user);

        identity.AddClaims(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
        identity.AddClaim(new Claim(JwtClaimsTypes.AccountId, user.AccountId.ToString()));
        identity.AddClaim(new Claim(JwtClaimsTypes.TimeZone, user.Account?.TimeZone ?? string.Empty));
        identity.AddClaim(new Claim(JwtClaimsTypes.FirstName, user.FirstName));
        identity.AddClaim(new Claim(JwtClaimsTypes.LastName, user.LastName));

        return identity;
    }
}

