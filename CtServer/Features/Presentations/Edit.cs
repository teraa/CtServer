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

            ModelChanges changes = new();

            changes.Set(nameof(entity.SectionId), entity.SectionId, request.Model.SectionId, x => entity.SectionId = x);
            changes.Set(nameof(entity.Title), entity.Title, request.Model.Title, x => entity.Title = x);
            changes.Set(nameof(entity.Authors), entity.Authors, request.Model.Authors, x => entity.Authors = x);
            changes.Set(nameof(entity.Description), entity.Description, request.Model.Description, x => entity.Description = x);
            changes.Set(nameof(entity.Position), entity.Position, request.Model.Position, x => entity.Position = x);
            changes.Set(nameof(entity.Duration), entity.Duration, TimeSpan.FromMinutes(request.Model.DurationMinutes), x => entity.Duration = x);

            if (!changes.Map.Any()) return new Success();

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Push.Notification
            (
                EventId: entity.Section.Event.Id,
                EventTitle: entity.Section.Event.Title,
                Type: NotificationType.PresentationEdited,
                Data: new { Id = entity.Id, Changes = changes.Map }
            )).ConfigureAwait(false);

            return new Success();
        }
    }
}
