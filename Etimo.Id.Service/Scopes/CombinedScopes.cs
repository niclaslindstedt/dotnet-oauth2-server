namespace Etimo.Id.Service.Scopes
{
    public class CombinedScopes
    {
        public const string ReadApplicationRole = ApplicationScopes.Read + "+" + RoleScopes.Read;
        public const string ReadRoleScope = RoleScopes.Read + "+" + ScopeScopes.Read;
        public const string ReadUserApplication = UserScopes.Read + "+" + ApplicationScopes.Read;
        public const string ReadUserRole = UserScopes.Read + "+" + RoleScopes.Read;
    }
}
