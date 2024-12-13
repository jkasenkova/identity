namespace Relay.IdentityServer.Infrastructure.Data.Entities;

public class User : IdentityUser<Guid>
{
    public Guid AccountId { get; set; }

    public bool IsPrimary { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;
}
