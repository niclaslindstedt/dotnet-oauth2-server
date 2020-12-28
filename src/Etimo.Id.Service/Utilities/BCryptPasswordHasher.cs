using Etimo.Id.Abstractions;
using Etimo.Id.Service.Settings;
using System;
using System.Diagnostics;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Etimo.Id.Service.Utilities
{
    /// <summary>
    ///     Encrypts text strings with BCrypt and finishes them up with base64 encoding.
    ///     https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html
    /// </summary>
    public class BCryptPasswordHasher : IPasswordHasher
    {
        private readonly int _workFactor;

        public BCryptPasswordHasher(CryptologySettings settings)
        {
            _workFactor = CalculateIdealWorkFactor(settings.MinimumHashingMilliseconds);
        }

        // When initializing this class using Dependency Injection, make sure it's used as a singleton
        // otherwise the work factor calculator will run needlessly every time it is instantiated.
        public BCryptPasswordHasher(int minimumHashingMilliseconds = 500)
        {
            _workFactor = CalculateIdealWorkFactor(minimumHashingMilliseconds);
        }

        public string Hash(string text)
        {
            string hash       = BCryptNet.HashPassword(text, _workFactor);
            string base64Hash = SerializeBase64(hash);
            return base64Hash;
        }

        public bool Verify(string text, string base64Hash)
        {
            string hash = DeserializeBase64(base64Hash);
            return BCryptNet.Verify(text, hash);
        }

        private static string SerializeBase64(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        private static string DeserializeBase64(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }

        private static int CalculateIdealWorkFactor(int minimumHashingMilliseconds)
        {
            var wf = 10;
            var sw = new Stopwatch();
            sw.Start();
            BCryptNet.HashPassword("hashing_benchmark", wf);
            sw.Stop();

            // https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#work-factors
            double hashTime = sw.Elapsed.TotalMilliseconds;
            while (hashTime < minimumHashingMilliseconds)
            {
                // +1 work factor means 2^(n+1) increased hashing time
                wf       += 1;
                hashTime *= 2;
            }

            return wf;
        }
    }
}
