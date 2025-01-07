namespace Relay.IdentityServer;

public static class Constants
{
    public const string iHandoverAccountName = "iHandover";
    public const string AdministratorRoleName = "Administrator";
    public static readonly Guid AdministratorRoleId = Guid.Parse("76dfdc04-e8ac-4ad3-9ddf-fca6266409e6");
    public const string RootAdminRoleName = "RootAdmin";
    public static readonly Guid RootAdminRoleId = Guid.Parse("24244bf6-2fce-4e67-9e72-7a6466b13238");
    public const string LineManagerRoleName = "LineManager";
    public static readonly Guid LineManagerRoleId = Guid.Parse("706aa928-3bd3-4fad-a3a0-ebbf81f6bd44");
    public const string UserRoleName = "User";
    public static readonly Guid UserRoleId = Guid.Parse("13415e5a-abb0-45b9-ab53-bafe53238ee4");

    public static string IdentityDefaultSchema = "Identity";
    public static string IdentityServerDefaultSchema = "IdentityServer";
}

public static class JwtClaimsTypes
{
    public const string AccountId = "AccountId";

    public const string TimeZone = "TimeZone";

    public const string FirstName = "FirstName";

    public const string LastName = "LastName";

    public const string RoleId = "RoleId";
}
