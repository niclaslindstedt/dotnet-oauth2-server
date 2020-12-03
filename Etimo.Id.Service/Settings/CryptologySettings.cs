namespace Etimo.Id.Service.Settings
{
    public class CryptologySettings
    {
        public int MinimumHashingMilliseconds { get; set; }
        public PasswordSettings PasswordSettings { get; set; }
    }

    public class PasswordSettings
    {
        public bool IncludeLowercase { get; set; }
        public bool IncludeUppercase { get; set; }
        public bool IncludeNumeric { get; set; }
        public bool IncludeSpecial { get; set; }
    }
}
