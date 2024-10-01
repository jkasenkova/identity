namespace Relay.IdentityServer.Models;

public record UserCreatingResponse
{
    public bool Succeeded { get; set; }

    public string? Error { get; set; }

    public string? UserId { get; set; }
}
