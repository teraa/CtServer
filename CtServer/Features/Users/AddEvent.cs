using CtServer.Data.Models;
using OneOf;

namespace CtServer.Features.Users;

public static class AddEvent
{
    public record Command
    (
        int UserId,
        Model Model
    ) : IRequest<OneOf<Success, NotFound, Fail>>;

    public record Model(int Id);

    public record Success;
    public record Fail(string Error);
    public record NotFound;

    public class Handler : IRequestHandler<Command, OneOf<Success, NotFound, Fail>>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<OneOf<Success, NotFound, Fail>> Handle(Command request, CancellationToken cancellationToken)
        {
            bool exists = await _ctx.UserEvents
                .AnyAsync(x => x.UserId == request.UserId && x.EventId == request.Model.Id, cancellationToken)
                .ConfigureAwait(false);

            if (exists)
                return new Fail("Already exists");

            var user = await _ctx.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                .ConfigureAwait(false);

            if (user is null) return new NotFound();

            var evt = await _ctx.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Model.Id, cancellationToken)
                .ConfigureAwait(false);

            if (evt is null) return new NotFound();

            var entity = new UserEvent
            {
                UserId = user.Id,
                EventId = evt.Id,
            };

            _ctx.UserEvents.Add(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new();
        }
    }
}
