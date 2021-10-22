using System;

#pragma warning disable CS8618
namespace CtServer.Data.Models
{
    public class Presentation
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; }
        public string[] Authors { get; set; }
        public string Description { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
        public string? Attachment { get; set; }
        public string? MainAuthorPhoto { get; set; }

        public Section Section { get; set; }
    }
}