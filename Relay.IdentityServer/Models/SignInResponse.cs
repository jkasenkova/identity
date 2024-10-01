namespace Relay.IdentityServer.Models;

public record SignInResponse
{
    public bool Succeeded { get; set; }

    public string? Error { get; set; }

    public string? UserId { get; set; }
}
