using OneOf;

namespace CtServer.Features.Users.Subscriptions;

public static class Delete
{
    public record Command
    (
        int UserId,
        Model Model
    ) : IRequest<OneOf<Success, NotFound>>;

    public record Model(string Endpoint);

    public class Handler : IRequestHandler<Command, OneOf<Success, NotFound>>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<OneOf<Success, NotFound>> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.Subscriptions
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.Endpoint == request.Model.Endpoint, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return new NotFound();

            _ctx.Subscriptions.Remove(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Success();
        }
    }
}
