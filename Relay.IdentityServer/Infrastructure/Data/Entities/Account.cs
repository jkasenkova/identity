namespace Relay.IdentityServer.Infrastructure.Data.Entities;

public class Account
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public virtual ICollection<User> Users { get; set; } = [];
}