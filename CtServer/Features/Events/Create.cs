using CtServer.Data.Models;

namespace CtServer.Features.Events;

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

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = new Event
            {
                Title = request.Model.Title,
                Description = request.Model.Description,
                StartAt = request.Model.StartAt,
                EndAt = request.Model.EndAt,
            };

            _ctx.Events.Add(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new(entity.Id);
        }
    }
}
