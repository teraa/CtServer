using CtServer.Results;
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

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<OneOf<Success, NotFound>> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.Presentations
                .AsQueryable()
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return new NotFound();

            entity.SectionId = request.Model.SectionId;
            entity.Title = request.Model.Title;
            entity.Authors = request.Model.Authors;
            entity.Description = request.Model.Description;
            entity.Position = request.Model.Position;
            entity.Duration = TimeSpan.FromMinutes(request.Model.DurationMinutes);
            entity.Attachment = request.Model.Attachment;
            entity.MainAuthorPhoto = request.Model.MainAuthorPhoto;

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Success();
        }
    }
}
