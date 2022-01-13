using CtServer.Data.Models;
using CtServer.Features.Notifications;

namespace CtServer.Features.Sections;

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
            var entity = new Section
            {
                EventId = request.Model.EventId,
                LocationId = request.Model.LocationId,
                Title = request.Model.Title,
                Chairs = request.Model.Chairs,
                StartAt = request.Model.StartAt,
                EndAt = request.Model.EndAt,
                BackgroundColor = request.Model.BackgroundColor,
            };

            _ctx.Sections.Add(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var evt = await _ctx.Events
                .AsNoTracking()
                .FirstAsync(x => x.Id == request.Model.EventId, cancellationToken)
                .ConfigureAwait(false);

            await _mediator.Publish(new Push.Notification
            (
                EventId: evt.Id,
                EventTitle: evt.Title,
                Type: NotificationType.SectionAdded,
                Data: new { Id = entity.Id, New = request.Model }
            )).ConfigureAwait(false);

            return new(entity.Id);
        }
    }
}
