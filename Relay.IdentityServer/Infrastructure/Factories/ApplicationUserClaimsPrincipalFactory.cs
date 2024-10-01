using Relay.IdentityServer.Infrastructure.Data.Entities;

namespace Relay.IdentityServer.Infrastructure.Factories;

public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
{
    public ApplicationUserClaimsPrincipalFactory(
        UserManager<User> userManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        var roles = await UserManager.GetRolesAsync(user);

        identity.AddClaims(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

        return identity;
    }
}

