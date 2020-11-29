// ReSharper disable InconsistentNaming

using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Api.OAuth
{
    public class AuthorizationCodeRequestForm
    {
        [Required]
        public string username { get; set; }

        [Required]
        public string password { get; set; }
    }
}
