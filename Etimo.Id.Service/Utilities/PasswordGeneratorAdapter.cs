using Etimo.Id.Abstractions;
using PasswordGenerator;

namespace Etimo.Id.Service.Utilities
{
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
