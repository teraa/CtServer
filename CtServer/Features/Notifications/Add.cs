using CtServer.Data.Models;
using OneOf;

namespace CtServer.Features.Notifications;

public static class Send
{
    public record Command
    (
        int EventId,
        string Message
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
            var evt = await _ctx.Events
                .FirstOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
                .ConfigureAwait(false);

            if (evt is null) return new NotFound();

            var data = new
            {
                New = new
                {
                    Message = request.Message,
                },
            };

            await _mediator.Publish(new Push.Notification(evt.Id, evt.Title, NotificationType.CustomMessage, data))
                .ConfigureAwait(false);

            return new Success();
        }
    }
}
