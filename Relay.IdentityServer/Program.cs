using Microsoft.AspNetCore.Identity;
using Relay.IdentityServer;
using Relay.IdentityServer.Infrastructure.Data;
using Relay.IdentityServer.Infrastructure.Data.Common;
using Relay.IdentityServer.Infrastructure.Data.Entities;
using Relay.IdentityServer.Infrastructure.Factories;
using Relay.IdentityServer.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

if (builder.Environment.EnvironmentName == "Production")
{
    builder.Configuration.AddAzureAppConfiguration(builder.Configuration["AzureAppConfiguration"]);
}

builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, dbContextOptionsBuilder) =>
{
    dbContextOptionsBuilder.UseNpgsql(
        builder.Configuration["AppSettings:PostgresConnection"],
        DbExtensions.NpgsqlOptionsAction);
});


builder.Services.AddIdentity<User, Role>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<Role>()
.AddApiEndpoints()
//.AddRoleManager<RoleManager<Role>>()
.AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

//builder.Services.AddScoped<RoleManager<Role>>();
//builder.Services.AddScoped<IEmailSender<User>, EmailSender>();
builder.Services.AddIdentityServer()
    .AddAspNetIdentity<User>()
    .AddConfigurationStore(configurationStoreOptions =>
    {
        configurationStoreOptions.ConfigureDbContext = dbContextOptionsBuilder => dbContextOptionsBuilder.UseNpgsql(
            builder.Configuration["AppSettings:PostgresConnection"],
            DbExtensions.NpgsqlOptionsAction);
        configurationStoreOptions.DefaultSchema = Constants.IdentityServerDefaultSchema;
    })
    .AddOperationalStore(operationalStoreOptions =>
    {
        operationalStoreOptions.ConfigureDbContext = dbContextOptionsBuilder => dbContextOptionsBuilder.UseNpgsql(
            builder.Configuration["AppSettings:PostgresConnection"],
            DbExtensions.NpgsqlOptionsAction);
        operationalStoreOptions.DefaultSchema = Constants.IdentityServerDefaultSchema;
    })
    .AddJwtBearerClientAuthentication();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // for testing purposes only
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapIdentityApi<User>();
app.UseIdentityServer();
app.UseAuthorization();
app.MapControllers();

using var scope = app.Services.CreateScope();

var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

var seedDatabase = new SeedDatabase(
    builder.Configuration,
    applicationDbContext,
    configurationDbContext,
    userManager,
    roleManager);

await seedDatabase.Seed();

app.Run();