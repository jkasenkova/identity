using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relay.IdentityServer.Infrastructure.Data;
using Relay.IdentityServer.Infrastructure.Data.Entities;
using Relay.IdentityServer.Models;
using System.Transactions;

namespace Relay.IdentityServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ApplicationDbContext dbContext,
    ILogger<AccountsController> logger) : ControllerBase
{
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly UserManager<User> _userManager = userManager;
    private readonly ILogger<AccountsController> _logger = logger;
    private readonly ApplicationDbContext _dbContext = dbContext;

    [AllowAnonymous]
    [HttpPost("sing-up")]
    public async Task<IActionResult> SingUpAsync([FromBody] SignUpRequest request, CancellationToken cancellationToken = default)
    {
        using (var scope = new TransactionScope())
        {
            var company = await CreateCompany(request, cancellationToken);

            var user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = true,
                AccountId = company.Id,
                IsPrimary = true
            };

            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Constants.LineManagerRoleName);

                return Ok(new SignUpResponse { Succeeded = true, CompanyId = company.Id, CompanyName = company.Name });
            }

            return BadRequest(new SignUpResponse { Succeeded = false, Error = string.Join(',', result.Errors) });
        }
    }

    [HttpPost("{accountId}/users")]
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
            AccountId = accountId,
            IsPrimary = false
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

    [HttpGet]
    public async Task<IActionResult> GetAccounts(CancellationToken cancellationToken = default)
    {
        var accounts = await _dbContext.Accounts
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
            TimeZoneId = x.TimeZoneId
        });
        return Ok(response);
    }

    private async Task<Account> CreateCompany(SignUpRequest account, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_dbContext.Accounts.Any(x => x.Name == account.CompanyName))
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
                TimeZoneId = Guid.NewGuid()
            };

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
