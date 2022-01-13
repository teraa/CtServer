namespace CtServer.Features.Admin;

public record Model
(
    IEnumerable<Events.ReadModel> Events,
    IEnumerable<Locations.ReadModel> Locations,
    IEnumerable<Sections.ReadModel> Sections,
    IEnumerable<Presentations.ReadModel> Presentations
);
