using System.Collections.Generic;

namespace Etimo.Id.Dtos
{
    public class JsonWebKeyDto
    {
        /// <summary>
        ///     The algorithm for the key.
        /// </summary>
        public string alg { get; set; }

        /// <summary>
        ///     The key type.
        /// </summary>
        public string kty { get; set; }

        /// <summary>
        ///     How the key was meant to be used (sig/enc).
        /// </summary>
        public string use { get; set; }

        /// <summary>
        ///     The x509 certificate chain.
        /// </summary>
        public List<string> x5c { get; set; } = new();

        /// <summary>
        ///     The exponent for a standard pem.
        /// </summary>
        public string e { get; set; }

        /// <summary>
        ///     The modulus for a standard pem.
        /// </summary>
        public string n { get; set; }

        /// <summary>
        ///     The unique identifier for the key.
        /// </summary>
        public string kid { get; set; }

        /// <summary>
        ///     The thumbprint of the x.509 cert (SHA-1 thumbprint).
        /// </summary>
        public string x5t { get; set; }
    }
}
