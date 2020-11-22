namespace Etimo.Id.Service.Constants
{
    public class Roles
    {
        public const string Admin = "admin";
        public const string User = "user";

        public static string[] InPrivilegeOrder() => new[] {Admin, User};
    }
}
