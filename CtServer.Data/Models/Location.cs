using System.Collections.Generic;

#pragma warning disable CS8618
namespace CtServer.Data.Models;

public class Location
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string Name { get; set; }

    public Event Event { get; set; }
    public ICollection<Section> Sections { get; set; }
}
