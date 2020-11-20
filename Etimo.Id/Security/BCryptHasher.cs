using System;
using System.Text;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Etimo.Id.Security
{
    /// <summary>
    /// Encrypts text strings with BCrypt and finishes them up with base64 encoding.
    /// </summary>
    public class BCryptHasher
    {
        public string Hash(string text)
        {
            var salt = BCryptNet.GenerateSalt();
            var hash = BCryptNet.HashPassword(text, salt);
            var base64Hash = SerializeBase64(hash);
            return base64Hash;
        }

        public bool Verify(string text, string base64Hash)
        {
            var hash = DeserializeBase64(base64Hash);
            return BCryptNet.Verify(text, hash);
        }

        private static string SerializeBase64(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        private static string DeserializeBase64(string base64String)
        {
            var bytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
