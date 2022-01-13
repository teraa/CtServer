using CtServer.Data.Models;
using CtServer.Features.Notifications;
using OneOf;

namespace CtServer.Features.Locations;

public static class Delete
{
    public record Command(int Id) : IRequest<OneOf<Success, NotFound>>;

    public class Handler : IRequestHandler<Command, OneOf<Success, NotFound>>
    {
        private readonly CtDbContext _ctx;
        private readonly IMediator _mediator;

        public Handler(CtDbContext ctx, IMediator mediator)
        {
            _ctx = ctx;
            _mediator = mediator;
        }

        public async Task<OneOf<Success, NotFound>> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.Locations
                .AsQueryable()
                .Include(x => x.Event)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return new NotFound();

            WriteModel oldModel = new
            (
                entity.EventId,
                entity.Name
            );

            _ctx.Locations.Remove(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Push.Notification
            (
                EventId: entity.Event.Id,
                EventTitle: entity.Event.Title,
                Type: NotificationType.LocationDeleted,
                Data: new { Id = entity.Id, Old = oldModel }
            )).ConfigureAwait(false);

            return new Success();
        }
    }
}
