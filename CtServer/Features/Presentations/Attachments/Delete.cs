using OneOf;

namespace CtServer.Features.Presentations.Attachments;

public static class Delete
{
    public record Command
    (
        int PresentationId
    ) : IRequest<OneOf<Success, NotFound>>;

    public class Handler : IRequestHandler<Command, OneOf<Success, NotFound>>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<OneOf<Success, NotFound>> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.Attachments
                .FirstOrDefaultAsync(x => x.PresentationId == request.PresentationId, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return new NotFound();

            if (File.Exists(entity.FilePath))
                File.Delete(entity.FilePath);

            _ctx.Attachments.Remove(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Success();
        }
    }
}
