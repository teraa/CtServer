#pragma warning disable CS8618
namespace CtServer.Data.Models;

public class Presentation
{
    public int Id { get; set; }
    public int SectionId { get; set; }
    public string Title { get; set; }
    public string[] Authors { get; set; }
    public string Description { get; set; }
    public int Position { get; set; }
    public TimeSpan Duration { get; set; }

    public Section Section { get; set; }
    public Attachment? Attachment { get; set; }
    public Photo? Photo { get; set; }
}
