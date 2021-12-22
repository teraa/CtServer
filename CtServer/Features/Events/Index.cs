namespace CtServer.Features.Events;

public static class Index
{
    public record Query : IRequest<Model[]>;

    public record Model
    (
        int Id,
        string Title,
        string Description,
        DateTimeOffset StartAt,
        DateTimeOffset EndAt
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

            var models = await ctx.Events
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new Model
                (
                    x.Id,
                    x.Title,
                    x.Description,
                    x.StartAt,
                    x.EndAt
                ))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            return models;
        }
    }
}
