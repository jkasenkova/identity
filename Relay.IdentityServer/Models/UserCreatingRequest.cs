namespace Relay.IdentityServer.Models;

public class UserCreatingRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
