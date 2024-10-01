using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Relay.IdentityServer.Infrastructure.Data.Common;

public static class DbExtensions
{
    public static void NpgsqlOptionsAction(NpgsqlDbContextOptionsBuilder npgsqlDbContextOptionsBuilder)
    {
        npgsqlDbContextOptionsBuilder.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
    }
}
