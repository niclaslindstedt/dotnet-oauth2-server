using Etimo.Id.Abstractions;
using PasswordGenerator;

namespace Etimo.Id.Service.Utilities
{
    /// <summary>
    /// Generates random passwords with different settings to meet the OWASP requirements
    /// Uses PasswordGenerator NuGet: https://github.com/prjseal/PasswordGenerator
    /// </summary>
    public class PasswordGeneratorAdapter : IPasswordGenerator
    {
        public string Generate(int length)
        {
            var pwd = new Password()
                .IncludeNumeric()
                .IncludeLowercase()
                .IncludeUppercase()
                .LengthRequired(length);
            return pwd.Next();
        }
    }
}
