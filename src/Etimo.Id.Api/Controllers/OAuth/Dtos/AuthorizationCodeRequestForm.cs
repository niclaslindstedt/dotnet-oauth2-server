// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Api.OAuth
{
    public class AuthorizationCodeRequestForm
    {
        [Required]
        [Unicode]
        public string username { get; set; }

        [Required]
        [Unicode]
        public string password { get; set; }
    }
}
