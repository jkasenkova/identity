using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relay.IdentityServer.Infrastructure.Data;
using Relay.IdentityServer.Infrastructure.Data.Entities;
using Relay.IdentityServer.Models;

namespace Relay.IdentityServer.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
public class UsersController(
    UserManager<User> userManager,
    ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        if (this.User.IsAuthenticated())
        {
            var storedUser = await userManager.GetUserAsync(this.User);
            var roles = await userManager.GetRolesAsync(storedUser!);

            return Ok(new UserResponse(storedUser!.Id, storedUser.FirstName, storedUser.LastName, [.. roles]));
        }

        return BadRequest();
    }

    [HttpPost]
    public async Task<IActionResult> AddUserToAccount(
        [FromBody] UserCreatingRequest request,
        CancellationToken cancellationToken = default)
    {
        var company = await dbContext.Accounts.FindAsync(request.AccountId, cancellationToken);

        if (company is null)
        {
            return BadRequest(new UserCreatingResponse { Succeeded = false, Error = "Company not found" });
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true,
            AccountId = request.AccountId,
            IsPrimary = false
        };

        var identityResult = await userManager.CreateAsync(user, request.Password);

        if (identityResult.Succeeded)
        {
            var role = dbContext.Roles.FirstOrDefault(x => x.Id == request.RoleId);

            if (role == null)
            {
                return BadRequest(new UserCreatingResponse { Succeeded = false, Error = $"Role with id: {request.RoleId} doesn't exist!" });
            }

            var userId = await userManager.GetUserIdAsync(user);
            await userManager.AddToRoleAsync(user, role.Name!);

            return Ok(new UserCreatingResponse { Succeeded = true, UserId = userId });
        }

        return BadRequest(new UserCreatingResponse { Succeeded = false, Error = string.Join(',', identityResult.Errors.Select(x => x.Description)) });
    }
}
