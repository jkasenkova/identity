using Relay.IdentityServer.Infrastructure.Data.Entities;

namespace Relay.IdentityServer.Infrastructure.Data.Common;

public class SeedDatabase(
    IConfiguration configuration,
    ApplicationDbContext dbContext,
    ConfigurationDbContext configurationDbContext,
    UserManager<User> userManager,
    RoleManager<Role> roleManager)
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly ConfigurationDbContext _configurationDbContext = configurationDbContext;
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<Role> _roleManager = roleManager;

    private readonly Guid RootCompanyId = Guid.Parse("79af820c-a5cb-4fab-a33a-22d76c10a8c7");

    public async Task Seed()
    {
        if (await _dbContext.Accounts.FindAsync(RootCompanyId) is null)
        {
            var company = new Account
            {
                Id = RootCompanyId,
                Name = Constants.iHandoverAccountName,
                Email = _configuration["IdentitySettings:AdminUserEmail"]!,
                CreatedDate = DateTime.UtcNow,
                LimitUsers = 100,
                Status = SubscriptionStatusType.Active,
                ActiveHandovers = 100,
                TimeZone = "UTC"
            };
            await _dbContext.Accounts.AddAsync(company);
            await _dbContext.SaveChangesAsync();
        }

        if (!await _roleManager.RoleExistsAsync(Constants.RootAdminRoleName))
        {
            await _roleManager.CreateAsync(new Role() { Name = Constants.RootAdminRoleName, Id = Constants.RootAdminRoleId });
        }

        if (!await _roleManager.RoleExistsAsync(Constants.AdministratorRoleName))
        {
            await _roleManager.CreateAsync(new Role() { Name = Constants.AdministratorRoleName, Id = Constants.AdministratorRoleId });
        }

        if (!await _roleManager.RoleExistsAsync(Constants.LineManagerRoleName))
        {
            await _roleManager.CreateAsync(new Role() { Name = Constants.LineManagerRoleName, Id = Constants.LineManagerRoleId });
        }

        if (!await _roleManager.RoleExistsAsync(Constants.UserRoleName))
        {
            await _roleManager.CreateAsync(new Role() { Name = Constants.UserRoleName, Id = Constants.UserRoleId });
        }

        if (await _userManager.FindByEmailAsync(_configuration["IdentitySettings:AdminUserEmail"]!) is null)
        {
            var account = await _dbContext.Accounts.FindAsync(RootCompanyId);

            var adminUser = new User
            {
                FirstName = "Admin",
                LastName = "iHandover",
                EmailConfirmed = true,
                UserName = _configuration["IdentitySettings:AdminUserEmail"],
                Email = _configuration["IdentitySettings:AdminUserEmail"],
                AccountId = account!.Id
            };

            var identity = await _userManager.CreateAsync(adminUser, _configuration["IdentitySettings:AdminUserPassword"]!);
            if (identity.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, Constants.RootAdminRoleName);
            }
        }

        if (!await _configurationDbContext.ApiResources.AnyAsync())
        {
            await _configurationDbContext.ApiResources.AddAsync(new ApiResource
            {
                Name = _configuration["AuthSettings:ApiResourceName"]!,
                DisplayName = "API",
                Scopes = [_configuration["AuthSettings:ScopeName"]!],
                UserClaims = { JwtClaimTypes.Role }
            }.ToEntity());

            await _configurationDbContext.SaveChangesAsync();
        }

        if (!await _configurationDbContext.ApiScopes.AnyAsync())
        {
            await _configurationDbContext.ApiScopes.AddAsync(new ApiScope
            {
                Name = _configuration["AuthSettings:ScopeName"]!,
                DisplayName = "API",
                UserClaims =
                {
                    JwtClaimTypes.Role,
                    JwtClaimTypes.Email,
                    JwtClaimTypes.Id,
                    JwtClaimsTypes.AccountId,
                    JwtClaimsTypes.TimeZone,
                    JwtClaimsTypes.FirstName,
                    JwtClaimsTypes.LastName
                }
            }.ToEntity());

            await _configurationDbContext.SaveChangesAsync();
        }

        if (!await _configurationDbContext.Clients.AnyAsync())
        {
            await _configurationDbContext.Clients.AddAsync(
                new Client
                {
                    ClientId = _configuration["AuthSettings:ClientId"]!,
                    ClientSecrets = new List<Secret> { new(_configuration["AuthSettings:ClientSecret"].Sha512()) },
                    ClientName = _configuration["AuthSettings:ClientName"],
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new List<string> { _configuration["AuthSettings:ScopeName"]! },
                    AlwaysSendClientClaims = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AccessTokenLifetime = 86400,
                    AbsoluteRefreshTokenLifetime = 86400,
                    AccessTokenType = AccessTokenType.Jwt,
                    RequireClientSecret = false,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AllowedCorsOrigins = { _configuration["AuthSettings:AllowedCorsOrigin"]! }
                }.ToEntity());

            await _configurationDbContext.SaveChangesAsync();
        }

        if (!await _configurationDbContext.IdentityResources.AnyAsync())
        {
            await _configurationDbContext.IdentityResources.AddRangeAsync(
                new IdentityResources.OpenId().ToEntity(),
                new IdentityResources.Profile().ToEntity(),
                new IdentityResources.Email().ToEntity());

            await _configurationDbContext.SaveChangesAsync();
        }
    }
}