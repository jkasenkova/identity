namespace Relay.IdentityServer;

public static class Constants
{
    public const string iHandoverAccountName = "iHandover";
    public const string AdministratorRoleName = "Administrator";
    public const string LineManagerRoleName = "LineManager";
    public const string UserRoleName = "User";

    public static string IdentityDefaultSchema = "Identity";
    public static string IdentityServerDefaultSchema = "IdentityServer";
}

public static class JwtClaimsTypes
{
    public const string AccountId = "AccountId";

    public const string TimeZone = "TimeZone";

    public const string FirstName = "FirstName";

    public const string LastName = "LastName";
}
