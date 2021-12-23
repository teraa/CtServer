namespace CtServer.Features.Users;

public static class Delete
{
    public record Command(int Id) : IRequest<Response?>;

    public record Response;

    public class Handler : IRequestHandler<Command, Response?>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Response?> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.Users
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return null;

            _ctx.Users.Remove(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new();
        }
    }
}
