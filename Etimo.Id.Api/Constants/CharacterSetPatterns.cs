// ReSharper disable All

namespace Etimo.Id.Api.Constants
{
    public static class CharacterSetPatterns
    {
        public const string VSCHAR = @"^[\x20-\x7E]$";
        public const string NQCHAR = @"^[\x21\x22-\x5B\x5D-\x7E]$";
        public const string NQSCHAR = @"^[\x20-\x21\x23-\x5B\x5D-\x7E]$";
        public const string UNICODECHARNOCRLF = @"^[\x09\x20-\x7E-\x80-\xD7FF\xE000-\xFFFD\x10000-\x10FFFF]$";
    }
}
