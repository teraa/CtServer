namespace CtServer.Features.Sections;

public static class Index
{
    public record Query : IRequest<Model[]>;

    public record Model
    (
        int Id,
        int EventId,
        int LocationId,
        string Title,
        string[] Chairs,
        DateTimeOffset StartAt,
        DateTimeOffset EndAt,
        int BackgroundColor
    );

    public class Handler : IRequestHandler<Query, Model[]>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public Handler(IServiceScopeFactory scopeFactory)
            => _scopeFactory = scopeFactory;

        public async Task<Model[]> Handle(Query request, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

            var models = await ctx.Sections
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new Model
                (
                    x.Id,
                    x.EventId,
                    x.LocationId,
                    x.Title,
                    x.Chairs,
                    x.StartAt,
                    x.EndAt,
                    x.BackgroundColor
                ))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            return models;
        }
    }
}
