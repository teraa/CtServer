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

            ModelChanges changes = new();

            changes.Set(nameof(entity.Title), entity.Title, request.Model.Title, x => entity.Title = x);
            changes.Set(nameof(entity.Description), entity.Description, request.Model.Description, x => entity.Description = x);
            changes.Set(nameof(entity.StartAt), entity.StartAt, request.Model.StartAt, x => entity.StartAt = x);
            changes.Set(nameof(entity.EndAt), entity.EndAt, request.Model.EndAt, x => entity.EndAt = x);

            if (!changes.Map.Any()) return new Success();

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Push.Notification
            (
                EventId: entity.Id,
                EventTitle: entity.Title,
                Type: NotificationType.EventEdited,
                Data: new { Id = entity.Id, Changes = changes.Map })
            ).ConfigureAwait(false);

            return new Success();
        }
    }
}
