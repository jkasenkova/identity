namespace Relay.IdentityServer.Infrastructure.Data.Entities;

public class Account
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public string Email { get; init; } = null!;

    public SubscriptionStatusType Status { get; init; }

    public DateTime CreatedDate { get; init; }

    public int LimitUsers { get; set; }

    public int ActiveHandovers { get; set; }

    public Guid TimeZoneId { get; set; }

    public virtual ICollection<User> Users { get; set; } = [];
}