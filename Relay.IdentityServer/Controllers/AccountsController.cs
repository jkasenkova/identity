using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relay.IdentityServer.Infrastructure.Data;
using Relay.IdentityServer.Infrastructure.Data.Entities;
using Relay.IdentityServer.Models;

namespace Relay.IdentityServer.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountsController(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    ApplicationDbContext dbContext,
    ILogger<AccountsController> logger) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("sing-up")]
    public async Task<IActionResult> SingUpAsync([FromBody] SignUpRequest request, CancellationToken cancellationToken = default)
    {
        var company = await CreateCompany(request, cancellationToken);

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true,
            AccountId = company.Id,
            IsPrimary = true
        };

        var identityResult = await userManager.CreateAsync(user, request.Password);

        if (identityResult.Succeeded)
        {
            var roleResult = await userManager.AddToRoleAsync(user, Constants.AdministratorRoleName);

            if (!roleResult.Succeeded)
            {
                return BadRequest(new SignUpResponse { Succeeded = false, Error = string.Join(',', roleResult.Errors) });
            }

            var storedUser = await userManager.FindByEmailAsync(request.Email);

            return Ok(new SignUpResponse
            {
                Succeeded = true,
                CompanyId = company.Id,
                CompanyName = company.Name,
                UserId = storedUser!.Id,
                RoleId = Constants.AdministratorRoleId
            });
        }

        return BadRequest(new SignUpResponse { Succeeded = false, Error = string.Join(',', identityResult.Errors) });
    }

    [HttpPost("{accountId}/users")]
    public async Task<IActionResult> AddUserToAccount(
        [FromBody] UserCreatingRequest request,
        [FromRoute] Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var company = await dbContext.Accounts.FindAsync(accountId, cancellationToken);

        if (company is null)
        {
            return BadRequest(new UserCreatingResponse { Succeeded = false, Error = "Company not found" });
        }

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true,
            AccountId = accountId,
            IsPrimary = false
        };

        var identityResult = await userManager.CreateAsync(user, request.Password);

        if (identityResult.Succeeded)
        {
            var roleExists = await roleManager.RoleExistsAsync(request.Role);

            if (!roleExists)
            {
                return BadRequest(new UserCreatingResponse { Succeeded = false, Error = $"Role with name: {request.Role} doesn't exist!" });
            }

            var userId = await userManager.GetUserIdAsync(user);
            await userManager.AddToRoleAsync(user, request.Role);

            return Ok(new UserCreatingResponse { Succeeded = true, UserId = userId });
        }

        return BadRequest(new UserCreatingResponse { Succeeded = false, Error = string.Join(',', identityResult.Errors.Select(x => x.Description)) });
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts(CancellationToken cancellationToken = default)
    {
        var accounts = await dbContext.Accounts
            .Include(x => x.Users.Where(u => u.IsPrimary))
            .ToListAsync(cancellationToken);

        var response = accounts.Select(x => new AccountResponse
        {
            Id = x.Id,
            Name = x.Name,
            Email = x.Users?.FirstOrDefault()?.Email ?? string.Empty,
            Status = SubscriptionStatus.Active,
            CreatedDate = x.CreatedDate,
            ActiveHandovers = x.ActiveHandovers,
            LimitUsers = x.LimitUsers,
            TimeZone = x.TimeZone
        });

        return Ok(response);
    }

    private async Task<Account> CreateCompany(SignUpRequest account, CancellationToken cancellationToken = default)
    {
        try
        {
            if (dbContext.Accounts.Any(x => x.Name == account.CompanyName))
            {
                throw new Exception("Company already exists.");
            }

            var company = new Account
            {
                Id = Guid.NewGuid(),
                Name = account.CompanyName,
                Status = SubscriptionStatusType.Inactive,
                CreatedDate = DateTime.UtcNow,
                Email = account.Email,
                TimeZone = account.Timezone
            };

            await dbContext.Accounts.AddAsync(company, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return company;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating company");
            throw;
        }
    }
}
