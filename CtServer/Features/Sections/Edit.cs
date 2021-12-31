using CtServer.Results;
using OneOf;

namespace CtServer.Features.Sections;

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

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<OneOf<Success, NotFound>> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.Sections
                .AsQueryable()
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return new NotFound();

            entity.EventId = request.Model.EventId;
            entity.LocationId = request.Model.LocationId;
            entity.Title = request.Model.Title;
            entity.Chairs = request.Model.Chairs;
            entity.StartAt = request.Model.StartAt;
            entity.EndAt = request.Model.EndAt;
            entity.BackgroundColor = request.Model.BackgroundColor;

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Success();
        }
    }
}
