using CtServer.Data.Models;
using CtServer.Features.Notifications;
using OneOf;

namespace CtServer.Features.Presentations;

public static class Delete
{
    public record Command
    (
        int Id
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
            var entity = await _ctx.Presentations
                .AsQueryable()
                .Include(x => x.Section)
                .ThenInclude(x => x.Event)
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return new NotFound();

            WriteModel oldModel = new
            (
                entity.SectionId,
                entity.Title,
                entity.Authors,
                entity.Description,
                entity.Position,
                (int)entity.Duration.TotalMinutes
            );

            _ctx.Presentations.Remove(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Push.Notification
            (
                EventId: entity.Section.Event.Id,
                EventTitle: entity.Section.Event.Title,
                Type: NotificationType.PresentationDeleted,
                Data: new { Id = entity.Id, Old = oldModel }
            )).ConfigureAwait(false);

            return new Success();
        }
    }
}
