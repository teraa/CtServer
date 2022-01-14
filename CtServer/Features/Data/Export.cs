using System.Text.Json;

namespace CtServer.Features.Data;

public static class Export
{
    public record Query : IRequest<Model>;

    public record Model
    (
        Stream Stream,
        string ContentType
    );

    public class Handler : IRequestHandler<Query, Model>
    {
        private readonly CtDbContext _ctx;
        private readonly JsonSerializerOptions _jsonOptions;

        public Handler(CtDbContext ctx, JsonSerializerOptions jsonOptions)
        {
            _ctx = ctx;
            _jsonOptions = jsonOptions;
        }

        public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
        {
            var events = await _ctx.Events
                .AsNoTracking()
                .Select(Events.ReadModel.FromEntity)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            var locations = await _ctx.Locations
                .AsNoTracking()
                .Select(Locations.ReadModel.FromEntity)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            var sections = await _ctx.Sections
                .AsNoTracking()
                .Select(Sections.ReadModel.FromEntity)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            var presentations = await _ctx.Presentations
                .AsNoTracking()
                .Select(Presentations.ReadModel.FromEntity)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            var dataModel = new Data.Model(events, locations, sections, presentations);

            var stream = new MemoryStream();

            await JsonSerializer.SerializeAsync(stream, dataModel, _jsonOptions, cancellationToken)
                .ConfigureAwait(false);

            stream.Seek(0, SeekOrigin.Begin);

            var model = new Model(stream, "application/json");

            return model;
        }
    }
}
