namespace CtServer.Features.Locations;

public static class Get
{
    public record Query(int Id) : IRequest<Model?>;

    public record Model
    (
        int Id,
        int EventId,
        string Name
    );

    public class Handler : IRequestHandler<Query, Model?>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public Handler(IServiceScopeFactory scopeFactory)
            => _scopeFactory = scopeFactory;

        public async Task<Model?> Handle(Query request, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

            var model = await ctx.Locations
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .Select(x => new Model
                (
                    x.Id,
                    x.EventId,
                    x.Name
                ))
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return model;
        }
    }
}
