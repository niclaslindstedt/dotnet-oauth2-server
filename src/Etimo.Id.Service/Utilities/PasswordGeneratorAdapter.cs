using Etimo.Id.Abstractions;
using PasswordGenerator;
using PasswordSettings = Etimo.Id.Settings.PasswordSettings;

namespace Etimo.Id.Service.Utilities
{
    /// <summary>
    ///     Generates random passwords with different settings to meet the OWASP requirements
    ///     Uses PasswordGenerator NuGet: https://github.com/prjseal/PasswordGenerator
    /// </summary>
    public class PasswordGeneratorAdapter : IPasswordGenerator
    {
        private readonly PasswordSettings _settings;

        public PasswordGeneratorAdapter(PasswordSettings settings)
        {
            _settings = settings;
        }

        public string Generate(int length)
        {
            IPassword password = new Password();

            if (_settings.IncludeLowercase) { password = password.IncludeLowercase(); }

            if (_settings.IncludeUppercase) { password = password.IncludeUppercase(); }

            if (_settings.IncludeNumeric) { password = password.IncludeNumeric(); }

            if (_settings.IncludeSpecial) { password = password.IncludeSpecial(); }

            return password.LengthRequired(length).Next();
        }
    }
}
