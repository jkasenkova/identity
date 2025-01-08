namespace Relay.IdentityServer.Models;

public class UserCreatingRequest
{
    public Guid AccountId { get; set; }

    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public Guid RoleId { get; set; }
}
