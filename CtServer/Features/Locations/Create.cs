using CtServer.Data.Models;
using CtServer.Features.Notifications;
using OneOf;

namespace CtServer.Features.Locations;

public static class Create
{
    public record Command
    (
        WriteModel Model
    ) : IRequest<OneOf<Success, Fail>>;

    public record Success(int Id);

    public class Handler : IRequestHandler<Command, OneOf<Success, Fail>>
    {
        private readonly CtDbContext _ctx;
        private readonly IMediator _mediator;

        public Handler(CtDbContext ctx, IMediator mediator)
        {
            _ctx = ctx;
            _mediator = mediator;
        }

        public async Task<OneOf<Success, Fail>> Handle(Command request, CancellationToken cancellationToken)
        {
            var evt = await _ctx.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Model.EventId, cancellationToken)
                .ConfigureAwait(false);

            if (evt is null)
                return new Fail("Invalid event ID.");

            var entity = new Location
            {
                EventId = request.Model.EventId,
                Name = request.Model.Name,
            };

            _ctx.Locations.Add(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Push.Notification
            (
                EventId: evt.Id,
                EventTitle: evt.Title,
                Type: NotificationType.LocationAdded,
                Data: new { Id = entity.Id, New = request.Model }
            )).ConfigureAwait(false);

            return new Success(entity.Id);
        }
    }
}
