using CtServer.Results;
using OneOf;

namespace CtServer.Features.Users.Events;

public static class Remove
{
    public record Command
    (
        int UserId,
        Model Model
    ) : IRequest<OneOf<Success, NotFound>>;

    public record Model(int Id);


    public class Handler : IRequestHandler<Command, OneOf<Success, NotFound>>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<OneOf<Success, NotFound>> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.UserEvents
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.EventId == request.Model.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return new NotFound();

            _ctx.UserEvents.Remove(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Success();
        }
    }
}
