using CtServer.Data.Models;
using CtServer.Features.Notifications;
using OneOf;

namespace CtServer.Features.Presentations;

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
                (int)entity.Duration.TotalMinutes,
                entity.Attachment,
                entity.MainAuthorPhoto
            );

            entity.SectionId = request.Model.SectionId;
            entity.Title = request.Model.Title;
            entity.Authors = request.Model.Authors;
            entity.Description = request.Model.Description;
            entity.Position = request.Model.Position;
            entity.Duration = TimeSpan.FromMinutes(request.Model.DurationMinutes);
            entity.Attachment = request.Model.Attachment;
            entity.MainAuthorPhoto = request.Model.MainAuthorPhoto;

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Push.Notification
            (
                EventId: entity.Section.Event.Id,
                EventTitle: entity.Section.Event.Title,
                Type: NotificationType.PresentationEdited,
                Data: new { Id = entity.Id, Old = oldModel, New = request.Model }
            )).ConfigureAwait(false);

            return new Success();
        }
    }
}
