using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relay.IdentityServer.Infrastructure.Data;
using Relay.IdentityServer.Infrastructure.Data.Entities;
using Relay.IdentityServer.Models;

namespace Relay.IdentityServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AccountController> _logger;
    private readonly ApplicationDbContext _dbContext;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ApplicationDbContext dbContext,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _dbContext = dbContext;
    }

    [AllowAnonymous]
    [HttpPost("sing-up")]
    public async Task<IActionResult> SingUpAsync([FromBody] SignUpRequest request, CancellationToken cancellationToken = default)
    {
        var company = await CreateCompany(request.CompanyName);

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true,
            AccountId = company.Id
        };

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            await _userManager.AddToRoleAsync(user, Constants.LineManagerRoleName);

            return Ok(new SignUpResponse { Succeeded = true, CompanyId = company.Id, CompanyName = company.Name });
        }

        return BadRequest(new SignUpResponse { Succeeded = false, Error = string.Join(',', result.Errors) });
    }

    [HttpPost("accounts/{accountId}/users")]
    public async Task<IActionResult> AddUserToAccount(
        [FromBody] UserCreatingRequest request,
        [FromRoute] Guid accountId,
        CancellationToken cancellationToken = default)
    {
        var company = _dbContext.Accounts.Find(accountId);

        if (company is null)
        {
            return BadRequest(new UserCreatingResponse { Succeeded = false, Error = "Company not found" });
        }

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true,
            AccountId = accountId
        };

        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            await _userManager.AddToRoleAsync(user, Constants.LineManagerRoleName);

            return Ok(new UserCreatingResponse { Succeeded = true, UserId = userId });
        }

        return BadRequest(new UserCreatingResponse { Succeeded = false, Error = string.Join(',', result.Errors.Select(x => x.Description)) });
    }

    [HttpGet("accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        var accounts = await _dbContext.Accounts.ToListAsync();
        return Ok(accounts);
    }

    private async Task<Account> CreateCompany(string companyName, CancellationToken cancellationToken = default)
    {
        try
        {
            var company = new Account { Id = Guid.NewGuid(), Name = companyName };

            await _dbContext.Accounts.AddAsync(company, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return company;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company");
            throw;
        }
    }
}
