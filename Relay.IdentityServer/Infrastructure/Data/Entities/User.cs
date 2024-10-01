namespace Relay.IdentityServer.Infrastructure.Data.Entities;

public class User : IdentityUser<Guid>
{
    public Guid AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;
}
