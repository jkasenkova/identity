namespace Relay.IdentityServer.Models;

public record SignUpResponse
{
    public bool Succeeded { get; set; }

    public string? Error { get; set; }

    public string? CompanyName { get; set; }

    public Guid? CompanyId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? RoleId { get; set; }
}
