using IdentityServer.Infrastructure.Data.Entities;

namespace IdentityServer.Infrastructure.Data.Common;

public class SeedDatabase
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;
    private readonly ConfigurationDbContext _configurationDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    private const string AdministratorRoleName = "Administrator";
    private const string LineManagerRoleName = "LineManager";
    private const string UserRoleName = "User";

    public SeedDatabase(
        IConfiguration configuration,
        ApplicationDbContext dbContext,
        ConfigurationDbContext configurationDbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _configurationDbContext = configurationDbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task Seed()
    {
        if (!await _roleManager.RoleExistsAsync(AdministratorRoleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(AdministratorRoleName));
        }

        if (!await _roleManager.RoleExistsAsync(LineManagerRoleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(LineManagerRoleName));
        }

        if (!await _roleManager.RoleExistsAsync(UserRoleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRoleName));
        }

        if (await _userManager.FindByEmailAsync(_configuration["IdentitySettings:AdminUserEmail"]!) is null)
        {
            var adminUser = new ApplicationUser
            {
                EmailConfirmed = true,
                UserName = "admin.user",
                Email = _configuration["IdentitySettings:AdminUserEmail"]
            };

            var identity = await _userManager.CreateAsync(adminUser, _configuration["IdentitySettings:AdminUserPassword"]!);
            if (identity.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, AdministratorRoleName);
            }
        }

        if (!await _configurationDbContext.ApiResources.AnyAsync())
        {
            await _configurationDbContext.ApiResources.AddAsync(new ApiResource
            {
                Name = _configuration["AuthSettings:ApiResourceName"]!,
                DisplayName = "API",
                Scopes = new List<string> { _configuration["AuthSettings:ScopeName"]! },
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
                UserClaims = { JwtClaimTypes.Role }
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
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AccessTokenType = AccessTokenType.Jwt,
                    RequireClientSecret = false,
                    UpdateAccessTokenClaimsOnRefresh = true,
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