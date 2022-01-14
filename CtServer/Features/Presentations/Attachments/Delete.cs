using CtServer.Options;
using Microsoft.Extensions.Options;
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
        private readonly StorageOptions _storageOptions;

        public Handler(CtDbContext ctx, IOptions<StorageOptions> storageOptions)
        {
            _ctx = ctx;
            _storageOptions = storageOptions.Value;
        }

        public async Task<OneOf<Success, NotFound>> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.Attachments
                .FirstOrDefaultAsync(x => x.PresentationId == request.PresentationId, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return new NotFound();

            string basePath = Path.GetFullPath(_storageOptions.AttachmentsPath);
            string fullPath = Path.Join(basePath, entity.FilePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            _ctx.Attachments.Remove(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Success();
        }
    }
}
