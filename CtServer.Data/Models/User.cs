#pragma warning disable CS8618

namespace CtServer.Data.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }

    public ICollection<UserEvent> UserEvents { get; set; }
    public ICollection<Event> Events { get; set; }
}
