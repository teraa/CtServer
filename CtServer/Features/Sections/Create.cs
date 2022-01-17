using CtServer.Data.Models;
using CtServer.Features.Notifications;
using OneOf;

namespace CtServer.Features.Sections;

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
            var evt = await _ctx.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Model.EventId, cancellationToken)
                .ConfigureAwait(false);

            if (evt is null)
                return new Fail("Invalid event ID.");

            if (request.Model.StartAt < evt.StartAt || request.Model.StartAt > evt.EndAt ||
                request.Model.EndAt < evt.StartAt || request.Model.EndAt > evt.EndAt)
            {
                return new Fail("Start and end times must be within event start and end times.");
            }

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

            await _mediator.Publish(new Push.Notification
            (
                EventId: evt.Id,
                EventTitle: evt.Title,
                Type: NotificationType.SectionAdded,
                Data: new { Id = entity.Id, New = request.Model }
            )).ConfigureAwait(false);

            return new Success(entity.Id);
        }
    }
}
