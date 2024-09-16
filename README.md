How to add migration to this solutiuon:

This solution has 3 dbcontext, so we need to add migration for each dbcontext and then update the database for each dbcontext.
Use the following commands to add migration and update the database:

dotnet ef migrations add 'Init' --context ApplicationDbContext --project IdentityServer --output-dir Migrations/ApplicationDb

dotnet ef migrations add 'Init' --context ConfigurationDbContext --project IdentityServer --output-dir Migrations/ConfigurationDb

dotnet ef migrations add 'Init' --context PersistedGrantDbContext --project IdentityServer --output-dir Migrations/PersistedGrantDb

dotnet ef database update --context ApplicationDbContext --project IdentityServer

dotnet ef database update --context ConfigurationDbContext --project IdentityServer

dotnet ef database update --context PersistedGrantDbContext --project IdentityServer