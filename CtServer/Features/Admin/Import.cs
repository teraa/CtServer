using System.Text.Json;
using CtServer.Data.Models;

namespace CtServer.Features.Admin;

public static class Import
{
    public record Command
    (
        IFormFile File
    ) : IRequest<Success>;

    public class Handler : IRequestHandler<Command, Success>
    {
        private readonly CtDbContext _ctx;
        private readonly JsonSerializerOptions _jsonOptions;

        public Handler(CtDbContext ctx, JsonSerializerOptions jsonOptions)
        {
            _ctx = ctx;
            _jsonOptions = jsonOptions;
        }

        public async Task<Success> Handle(Command request, CancellationToken cancellationToken)
        {
            Model data;

            using (var stream = request.File.OpenReadStream())
            {
                data = await JsonSerializer.DeserializeAsync<Model>(stream, _jsonOptions, cancellationToken)
                    .ConfigureAwait(false) ?? null!;
            }

            var events = data.Events
                .Select(x => new Event
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    StartAt = x.StartAt,
                    EndAt = x.EndAt,
                });

            var locations = data.Locations
                .Select(x => new Location
                {
                    Id = x.Id,
                    EventId = x.EventId,
                    Name = x.Name,
                });

            var sections = data.Sections
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

            var presentations = data.Presentations
                .Select(x => new Presentation
                {
                    Id = x.Id,
                    SectionId = x.SectionId,
                    Title = x.Title,
                    Authors = x.Authors,
                    Description = x.Description,
                    Position = x.Position,
                    Duration = TimeSpan.FromMinutes(x.DurationMinutes),
                });

            var oldEvents = await _ctx.Events
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            _ctx.Events.RemoveRange(oldEvents);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            _ctx.Events.AddRange(events);
            _ctx.Locations.AddRange(locations);
            _ctx.Sections.AddRange(sections);
            _ctx.Presentations.AddRange(presentations);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Success();
        }
    }
}
