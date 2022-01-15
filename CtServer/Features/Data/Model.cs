namespace CtServer.Features.Data;

public record Model
(
    IEnumerable<Events.ReadModel> Events,
    IEnumerable<Locations.ReadModel> Locations,
    IEnumerable<Sections.ReadModel> Sections,
    IEnumerable<Presentations.ReadModel> Presentations
);
