using System.Collections.Generic;
using System.Linq;

namespace Etimo.Id.Security
{
    public class InbuiltScopes
    {
        public static List<string> All { get => Application.Concat(AuditLog).Concat(Role).Concat(Scope).Concat(User).ToList(); }

        public static List<string> Read {
            get
                => new()
                {
                    ApplicationScopes.Read,
                    AuditLogScopes.Read,
                    RoleScopes.Read,
                    ScopeScopes.Read,
                    UserScopes.Read,
                };
        }

        public static List<string> Write {
            get
                => new()
                {
                    ApplicationScopes.Write,
                    RoleScopes.Write,
                    ScopeScopes.Write,
                    UserScopes.Write,
                };
        }

        public static List<string> Admin {
            get
                => new()
                {
                    ApplicationScopes.Admin,
                    AuditLogScopes.Admin,
                    RoleScopes.Admin,
                    ScopeScopes.Admin,
                    UserScopes.Admin,
                };
        }

        public static List<string> Application {
            get
                => new()
                {
                    ApplicationScopes.Read,
                    ApplicationScopes.Write,
                    ApplicationScopes.Admin,
                };
        }

        public static List<string> AuditLog {
            get
                => new()
                {
                    AuditLogScopes.Read,
                    AuditLogScopes.Admin,
                };
        }

        public static List<string> Role {
            get
                => new()
                {
                    RoleScopes.Read,
                    RoleScopes.Write,
                    RoleScopes.Admin,
                };
        }

        public static List<string> Scope {
            get
                => new()
                {
                    ScopeScopes.Read,
                    ScopeScopes.Write,
                    ScopeScopes.Admin,
                };
        }

        public static List<string> User {
            get
                => new()
                {
                    UserScopes.Read,
                    UserScopes.Write,
                    UserScopes.Admin,
                };
        }
    }
}
