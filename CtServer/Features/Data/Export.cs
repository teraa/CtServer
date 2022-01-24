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
                .Include(x => x.Sections)
                .ThenInclude(x => x.Presentations)
                .Include(x => x.Locations)
                .Include(x => x.Notifications)
                .ToArrayAsync(cancellationToken);

            var stream = new MemoryStream();

            await JsonSerializer.SerializeAsync(stream, events, _jsonOptions, cancellationToken)
                .ConfigureAwait(false);

            stream.Seek(0, SeekOrigin.Begin);

            var model = new Model(stream, "application/json");

            return model;
        }
    }
}
