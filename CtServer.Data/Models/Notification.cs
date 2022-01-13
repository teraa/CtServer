#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations.Schema;

namespace CtServer.Data.Models;

public class Notification
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public NotificationType Type { get; set; }
    public string Data { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Event Event { get; set; }
}

public enum NotificationType
{
    EventAdded,
    EventEdited,
    EventDeleted,
    LocationAdded,
    LocationEdited,
    LocationDeleted,
    PresentationAdded,
    PresentationEdited,
    PresentationDeleted,
    SectionAdded,
    SectionEdited,
    SectionDeleted,
}
