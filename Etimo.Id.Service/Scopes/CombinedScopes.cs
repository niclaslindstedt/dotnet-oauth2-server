namespace Etimo.Id.Service.Scopes
{
    public class CombinedScopes
    {
        public const string ReadApplicationRole = ApplicationScopes.Read + "+" + RoleScopes.Read;
    }
}
