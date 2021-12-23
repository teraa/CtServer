#pragma warning disable CS8618
namespace CtServer.Data.Models;

public class Section
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int LocationId { get; set; }
    public string Title { get; set; }
    public string[] Chairs { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public int BackgroundColor { get; set; }

    public Event Event { get; set; }
    public Location Location { get; set; }
    public ICollection<Presentation> Presentations { get; set; }
}
