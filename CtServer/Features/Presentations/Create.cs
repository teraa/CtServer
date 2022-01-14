using CtServer.Data.Models;
using CtServer.Features.Notifications;

namespace CtServer.Features.Presentations;

public static class Create
{
    public record Command
    (
        WriteModel Model
    ) : IRequest<Response>;

    public record Response(int Id);

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly CtDbContext _ctx;
        private readonly IMediator _mediator;

        public Handler(CtDbContext ctx, IMediator mediator)
        {
            _ctx = ctx;
            _mediator = mediator;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = new Presentation
            {
                SectionId = request.Model.SectionId,
                Title = request.Model.Title,
                Authors = request.Model.Authors,
                Description = request.Model.Description,
                Position = request.Model.Position,
                Duration = TimeSpan.FromMinutes(request.Model.DurationMinutes),
                MainAuthorPhoto = request.Model.MainAuthorPhoto,
            };

            _ctx.Presentations.Add(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var evt = await _ctx.Sections
                .AsNoTracking()
                .Where(x => x.Id == request.Model.SectionId)
                .Select(x => x.Event)
                .FirstAsync(cancellationToken)
                .ConfigureAwait(false);

            await _mediator.Publish(new Push.Notification
            (
                EventId: evt.Id,
                EventTitle: evt.Title,
                Type: NotificationType.PresentationAdded,
                Data: new { Id = entity.Id, New = request.Model }
            )).ConfigureAwait(false);

            return new(entity.Id);
        }
    }
}
