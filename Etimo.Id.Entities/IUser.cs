using System;

namespace Etimo.Id.Entities
{
    public interface IUser
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
    }
}
