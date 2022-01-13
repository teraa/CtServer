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

            entity.Title = request.Model.Title;
            entity.Description = request.Model.Description;
            entity.StartAt = request.Model.StartAt;
            entity.EndAt = request.Model.EndAt;

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Push.Notification(request.Id, request.Model.Title, NotificationType.EventEdited, new { Old = oldModel, New = request.Model }))
                .ConfigureAwait(false);

            return new Success();
        }
    }
}
