namespace Relay.IdentityServer.Models;

public record UserResponse(Guid Id, string FirstName, string LastName, string[] Role);
