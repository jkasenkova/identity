using Newtonsoft.Json;
using Relay.IdentityServer.Infrastructure.Data.Entities;

namespace Relay.IdentityServer.Infrastructure.Factories;

public class ApplicationUserClaimsPrincipalFactory(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<User>(userManager, optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        var roleNames = await UserManager.GetRolesAsync(user);
        var roles = roleManager.Roles.Where(x => roleNames.Contains(x.Name!)).ToList();
        var roleClaims = roles
            .Select(role => JsonConvert.SerializeObject(new { RoleName = role.Name, RoleId = role.Id }))
            .Select(x => new Claim(JwtClaimTypes.Role, x));

        identity.AddClaims(roleClaims);
        identity.AddClaim(new Claim(JwtClaimsTypes.AccountId, user.AccountId.ToString()));
        identity.AddClaim(new Claim(JwtClaimsTypes.TimeZone, user.Account?.TimeZone ?? string.Empty));
        identity.AddClaim(new Claim(JwtClaimsTypes.FirstName, user.FirstName));
        identity.AddClaim(new Claim(JwtClaimsTypes.LastName, user.LastName));

        return identity;
    }
}

