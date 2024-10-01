using Relay.IdentityServer.Infrastructure.Data.Entities;

namespace Relay.IdentityServer.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, Role, Guid>(options)
{
    public DbSet<Account> Accounts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Constants.IdentityDefaultSchema);

        modelBuilder.Entity<Account>(x =>
        {
            x.HasKey(x => x.Id);

            x.HasMany(x => x.Users)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId);
        });
    }
}