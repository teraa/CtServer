namespace CtServer.Features.Locations;

public static class Delete
{
    public record Command(int Id) : IRequest<Response?>;

    public record Response;

    public class Handler : IRequestHandler<Command, Response?>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public Handler(IServiceScopeFactory scopeFactory)
            => _scopeFactory = scopeFactory;

        public async Task<Response?> Handle(Command request, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

            var entity = await ctx.Locations
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return null;

            ctx.Locations.Remove(entity);

            await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new();
        }
    }
}
