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
    EventEdited = 1,
    LocationAdded = 3,
    LocationEdited = 4,
    LocationDeleted = 5,
    PresentationAdded = 6,
    PresentationEdited = 7,
    PresentationDeleted = 8,
    SectionAdded = 9,
    SectionEdited = 10,
    SectionDeleted = 11,
}
