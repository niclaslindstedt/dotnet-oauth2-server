namespace Etimo.Id.Service.Scopes
{
    public class CombinedScopes
    {
        public const string ReadApplicationRole = ApplicationScopes.Read + "+" + RoleScopes.Read;
        public const string ReadRoleScope = RoleScopes.Read + "+" + ScopeScopes.Read;
    }
}
