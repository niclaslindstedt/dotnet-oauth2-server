using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etimo.Id.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid RefreshTokenId { get; set; }
        public string EncryptedSecret { get; set; }

        [ForeignKey(nameof(User))]
        public virtual Guid UserId { get; set; }
    }
}
