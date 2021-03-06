using CtServer.Data.Models;
using CtServer.Features.Notifications;
using OneOf;

namespace CtServer.Features.Events;

public static class Edit
{
    public record Command
    (
        int Id,
        WriteModel Model
    ) : IRequest<OneOf<Success, NotFound>>;

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
            var entity = await _ctx.Events
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return new NotFound();

            WriteModel oldModel = new
            (
                entity.Title,
                entity.Description,
                entity.StartAt,
                entity.EndAt
            );

            bool updated = false;

            updated |= Extensions.TryUpdate(entity.Title, request.Model.Title, x => entity.Title = x);
            updated |= Extensions.TryUpdate(entity.Description, request.Model.Description, x => entity.Description = x);
            updated |= Extensions.TryUpdate(entity.StartAt, request.Model.StartAt, x => entity.StartAt = x);
            updated |= Extensions.TryUpdate(entity.EndAt, request.Model.EndAt, x => entity.EndAt = x);

            if (!updated) return new Success();

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Push.Notification
            (
                EventId: entity.Id,
                EventTitle: entity.Title,
                Type: NotificationType.EventEdited,
                Data: new { Id = entity.Id, Old = oldModel, New = request.Model })
            ).ConfigureAwait(false);

            return new Success();
        }
    }
}
