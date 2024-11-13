using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Relay.IdentityServer;
using Relay.IdentityServer.Infrastructure.Data;
using Relay.IdentityServer.Infrastructure.Data.Common;
using Relay.IdentityServer.Infrastructure.Data.Entities;
using Relay.IdentityServer.Infrastructure.Factories;

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
        npgsqlDbContextOptionsBuilder =>
        {
            npgsqlDbContextOptionsBuilder.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
            npgsqlDbContextOptionsBuilder.MigrationsHistoryTable("_EFMigrationsHistory", Constants.IdentityDefaultSchema);
        });
});


builder.Services.AddIdentity<User, Role>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<Role>()
.AddApiEndpoints()
.AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddIdentityServer()
    .AddAspNetIdentity<User>()
    .AddConfigurationStore(configurationStoreOptions =>
    {
        configurationStoreOptions.ConfigureDbContext = dbContextOptionsBuilder => dbContextOptionsBuilder.UseNpgsql(
            builder.Configuration["AppSettings:PostgresConnection"],
            npgsqlDbContextOptionsBuilder =>
            {
                npgsqlDbContextOptionsBuilder.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                npgsqlDbContextOptionsBuilder.MigrationsHistoryTable("_EFMigrationsHistory", Constants.IdentityServerDefaultSchema);
            });
        configurationStoreOptions.DefaultSchema = Constants.IdentityServerDefaultSchema;
    })
    .AddOperationalStore(operationalStoreOptions =>
    {
        operationalStoreOptions.ConfigureDbContext = dbContextOptionsBuilder => dbContextOptionsBuilder.UseNpgsql(
            builder.Configuration["AppSettings:PostgresConnection"],
            npgsqlDbContextOptionsBuilder =>
            {
                npgsqlDbContextOptionsBuilder.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                npgsqlDbContextOptionsBuilder.MigrationsHistoryTable("_EFMigrationsHistory", Constants.IdentityServerDefaultSchema);
            });
        operationalStoreOptions.DefaultSchema = Constants.IdentityServerDefaultSchema;
    })
    .AddJwtBearerClientAuthentication();

builder.Services.AddAuthentication()
    .AddCookie(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});

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
app.UseCors();
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