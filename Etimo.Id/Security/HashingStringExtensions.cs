namespace Etimo.Id.Security
{
    public static class HashingStringExtensions
    {
        public static string BCrypt(this string text)
        {
            return new BCryptHasher().Hash(text);
        }

        public static bool BCryptVerify(this string text, string base64Hash)
        {
            return new BCryptHasher().Verify(text, base64Hash);
        }
    }
}
