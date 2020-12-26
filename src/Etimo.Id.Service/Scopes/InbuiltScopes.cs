using System.Collections.Generic;
using System.Linq;

namespace Etimo.Id.Service.Scopes
{
    public class InbuiltScopes
    {
        public static List<string> All => Application.Concat(AuditLog).Concat(Role).Concat(Scope).Concat(User).ToList();

        public static List<string> Read => new List<string> {
            ApplicationScopes.Read,
            AuditLogScopes.Read,
            RoleScopes.Read,
            ScopeScopes.Read,
            UserScopes.Read
        };

        public static List<string> Write => new List<string> {
            ApplicationScopes.Write,
            RoleScopes.Write,
            ScopeScopes.Write,
            UserScopes.Write
        };

        public static List<string> Admin => new List<string> {
            ApplicationScopes.Admin,
            AuditLogScopes.Admin,
            RoleScopes.Admin,
            ScopeScopes.Admin,
            UserScopes.Admin
        };

        public static List<string> Application => new List<string> {
            ApplicationScopes.Read,
            ApplicationScopes.Write,
            ApplicationScopes.Admin
        };

        public static List<string> AuditLog => new List<string> {
            AuditLogScopes.Read,
            AuditLogScopes.Admin
        };

        public static List<string> Role => new List<string> {
            RoleScopes.Read,
            RoleScopes.Write,
            RoleScopes.Admin
        };

        public static List<string> Scope => new List<string> {
            ScopeScopes.Read,
            ScopeScopes.Write,
            ScopeScopes.Admin
        };

        public static List<string> User => new List<string> {
            UserScopes.Read,
            UserScopes.Write,
            UserScopes.Admin
        };
    }
}
