using System;

namespace Etimo.Id.Entities.Abstractions
{
    public interface IUser
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
    }
}
