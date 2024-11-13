using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Relay.IdentityServer.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new { id = Guid.NewGuid(), firstName = "fname", lastName = "lname", role = "role" });
    }
}
