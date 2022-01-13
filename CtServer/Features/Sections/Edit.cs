using CtServer.Data.Models;
using CtServer.Features.Notifications;
using OneOf;

namespace CtServer.Features.Sections;

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
            var entity = await _ctx.Sections
                .AsQueryable()
                .Include(x => x.Event)
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return new NotFound();

            WriteModel oldModel = new
            (
                entity.EventId,
                entity.LocationId,
                entity.Title,
                entity.Chairs,
                entity.StartAt,
                entity.EndAt,
                entity.BackgroundColor
            );

            entity.EventId = request.Model.EventId;
            entity.LocationId = request.Model.LocationId;
            entity.Title = request.Model.Title;
            entity.Chairs = request.Model.Chairs;
            entity.StartAt = request.Model.StartAt;
            entity.EndAt = request.Model.EndAt;
            entity.BackgroundColor = request.Model.BackgroundColor;

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Push.Notification
            (
                EventId: entity.Event.Id,
                EventTitle: entity.Event.Title,
                Type: NotificationType.SectionEdited,
                Data: new { Id = entity.Id, Old = oldModel, New = request.Model }
            )).ConfigureAwait(false);

            return new Success();
        }
    }
}
