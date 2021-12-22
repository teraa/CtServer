namespace CtServer.Features.Events;

public static class GetLocations
{
    public record Query(int EventId) : IRequest<Model[]?>;

    public record Model
    (
        int Id,
        string Name
    );

    public class Handler : IRequestHandler<Query, Model[]?>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public Handler(IServiceScopeFactory scopeFactory)
            => _scopeFactory = scopeFactory;

        public async Task<Model[]?> Handle(Query request, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

            var models = await ctx.Locations
                .AsNoTracking()
                .Where(x => x.EventId == request.EventId)
                .OrderBy(x => x.Id)
                .Select(x => new Model
                (
                    x.Id,
                    x.Name
                ))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!models.Any())
            {
                var exists = await ctx.Events
                    .AnyAsync(x => x.Id == request.EventId, cancellationToken)
                    .ConfigureAwait(false);

                if (!exists)
                    return null;
            }

            return models;
        }
    }
}
