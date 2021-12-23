namespace CtServer.Features.Users;

public static class RemoveEvent
{
    public record Command
    (
        int UserId,
        Model Model
    ) : IRequest<Response?>;

    public record Model(int Id);

    public record Response;

    public class Handler : IRequestHandler<Command, Response?>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Response?> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.UserEvents
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.EventId == request.Model.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return null;

            _ctx.UserEvents.Remove(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new();
        }
    }
}
