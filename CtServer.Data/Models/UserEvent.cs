#pragma warning disable CS8618
namespace CtServer.Data.Models;

public class UserEvent
{
    public int UserId { get; set; }
    public int EventId { get; set; }

    public User User { get; set; }
    public Event Event { get; set; }
}
