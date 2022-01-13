namespace CtServer.Features.Admin;

public static class Export
{
    public record Query : IRequest<Model>;

    public record Model
    (
        IEnumerable<Events.ReadModel> Events,
        IEnumerable<Locations.ReadModel> Locations,
        IEnumerable<Sections.ReadModel> Sections,
        IEnumerable<Presentations.ReadModel> Presentations
    );

    public class Handler : IRequestHandler<Query, Model>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

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

            var model = new Model(events, locations, sections, presentations);

            return model;
        }
    }
}
