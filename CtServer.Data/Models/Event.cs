#pragma warning disable CS8618
namespace CtServer.Data.Models;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }

    public ICollection<Section> Sections { get; set; }
    public ICollection<Location> Locations { get; set; }
    public ICollection<UserEvent> UserEvents { get; set; }
    public ICollection<User> Users { get; set; }
    public ICollection<Notification> Notifications { get; set; }
}
