using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relay.IdentityServer.Infrastructure.Data.Entities;
using Relay.IdentityServer.Models;

namespace Relay.IdentityServer.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController(
    UserManager<User> userManager) : ControllerBase
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
}
