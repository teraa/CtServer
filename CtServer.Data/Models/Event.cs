using System;
using System.Collections.Generic;

#pragma warning disable CS8618
namespace CtServer.Data.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }

        public ICollection<Section> Sections { get; set; }
    }
}