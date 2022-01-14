using CtServer.Data.Models;

namespace CtServer.Features.Admin;

public static class Import
{
    public record Command
    (
        Model Model
    ) : IRequest<Success>;

    public class Handler : IRequestHandler<Command, Success>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Success> Handle(Command request, CancellationToken cancellationToken)
        {
            var oldEvents = await _ctx.Events
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            _ctx.Events.RemoveRange(oldEvents);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var events = request.Model.Events
                .Select(x => new Event
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    StartAt = x.StartAt,
                    EndAt = x.EndAt,
                });

            var locations = request.Model.Locations
                .Select(x => new Location
                {
                    Id = x.Id,
                    EventId = x.EventId,
                    Name = x.Name,
                });

            var sections = request.Model.Sections
                .Select(x => new Section
                {
                    Id = x.Id,
                    EventId = x.EventId,
                    LocationId = x.LocationId,
                    Title = x.Title,
                    Chairs = x.Chairs,
                    StartAt = x.StartAt,
                    EndAt = x.EndAt,
                    BackgroundColor = x.BackgroundColor,
                });

            var presentations = request.Model.Presentations
                .Select(x => new Presentation
                {
                    Id = x.Id,
                    SectionId = x.SectionId,
                    Title = x.Title,
                    Authors = x.Authors,
                    Description = x.Description,
                    Position = x.Position,
                    Duration = TimeSpan.FromMinutes(x.DurationMinutes),
                    MainAuthorPhoto = x.MainAuthorPhoto,
                });

            _ctx.Events.AddRange(events);
            _ctx.Locations.AddRange(locations);
            _ctx.Sections.AddRange(sections);
            _ctx.Presentations.AddRange(presentations);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Success();
        }
    }
}