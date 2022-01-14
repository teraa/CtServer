#pragma warning disable CS8618
namespace CtServer.Data.Models;

public class Attachment
{
    public int Id { get; set; }
    public int PresentationId { get; set; }
    public string FilePath { get; set; }

    public Presentation Presentation { get; set; }
}
