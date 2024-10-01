namespace Relay.IdentityServer.Models;

public record AccountResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public SubscriptionStatus Status { get; set; }

    public DateTime CreatedDate { get; set; }

    public int LimitUsers { get; set; }

    public int ActiveHandovers { get; set; }

    public Guid TimeZoneId { get; set; }
}
