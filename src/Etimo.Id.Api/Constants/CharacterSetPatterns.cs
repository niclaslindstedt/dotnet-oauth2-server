// ReSharper disable All

namespace Etimo.Id.Api.Constants
{
    public static class CharacterSetPatterns
    {
        public const string VSCHAR            = "^[\x20-\x7E]+$";                       // ASCII
        public const string NQCHAR            = "^[\x21\x23-\\\x5B\\\x5D-\\\x7E]+$";    // ASCII except \ and " and <space>
        public const string NQSCHAR           = "^[\x20-\x21\x23-\\\x5B\\\x5D-\x7E]+$"; // ASCII except \ and "
        public const string UNICODECHARNOCRLF = "^[\x09\x20-\x7E-\x80-\xD7FF\xE000-\xFFFD\x10000-\x10FFFF]+$";
        public const string NUMALPHA          = "^[0-9A-Za-z]+$";
    }
}
