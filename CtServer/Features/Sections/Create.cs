using CtServer.Data.Models;

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

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

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

            return new(entity.Id);
        }
    }
}
