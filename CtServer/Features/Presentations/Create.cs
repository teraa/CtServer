using CtServer.Data.Models;
using CtServer.Features.Notifications;
using OneOf;

namespace CtServer.Features.Presentations;

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
            var section = await _ctx.Sections
                .AsNoTracking()
                .Include(x => x.Event)
                .Where(x => x.Id == request.Model.SectionId)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (section is null)
                return new Fail("Invalid section ID.");

            var entity = new Presentation
            {
                SectionId = request.Model.SectionId,
                Title = request.Model.Title,
                Authors = request.Model.Authors,
                Description = request.Model.Description,
                Position = request.Model.Position,
                Duration = TimeSpan.FromMinutes(request.Model.DurationMinutes)
            };

            _ctx.Presentations.Add(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Push.Notification
            (
                EventId: section.Event.Id,
                EventTitle: section.Event.Title,
                Type: NotificationType.PresentationAdded,
                Data: new { Id = entity.Id, New = request.Model }
            )).ConfigureAwait(false);

            return new Success(entity.Id);
        }
    }
}
