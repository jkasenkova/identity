namespace Relay.IdentityServer.Models;

public record SignInRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
