using Etimo.Id.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace Etimo.Id.Service.Utilities
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private readonly RNGCryptoServiceProvider _random = new RNGCryptoServiceProvider();

        public string Generate(int length)
        {
            var res = new StringBuilder();
            var random = new byte[1];
            using (_random)
            {
                while (0 < length--)
                {
                    var randomCharacter = '\0';
                    do
                    {
                        _random.GetBytes(random);
                        randomCharacter = (char)(random[0] % 92 + 33);
                    } while (!char.IsLetterOrDigit(randomCharacter));
                    res.Append(randomCharacter);
                }
            }
            return res.ToString();
        }
    }
}
