using CtServer.Data.Models;
using CtServer.Options;
using Microsoft.Extensions.Options;
using OneOf;

namespace CtServer.Features.Presentations.Attachments;

public static class Upload
{
    public record Command
    (
        int PresentationId,
        IFormFile File
    ) : IRequest<OneOf<Success, Fail>>;

    public class Handler : IRequestHandler<Command, OneOf<Success, Fail>>
    {
        private readonly CtDbContext _ctx;
        private readonly StorageOptions _storageOptions;

        public Handler(CtDbContext ctx, IOptions<StorageOptions> storageOptions)
        {
            _ctx = ctx;
            _storageOptions = storageOptions.Value;
        }

        public async Task<OneOf<Success, Fail>> Handle(Command request, CancellationToken cancellationToken)
        {
            string randomFileName = Path.GetRandomFileName();
            string basePath = Path.GetFullPath(_storageOptions.AttachmentsPath);
            string fullPath = Path.Join(basePath, randomFileName);

            var currentAttachment = await _ctx.Attachments
                .FirstOrDefaultAsync(x => x.PresentationId == request.PresentationId, cancellationToken)
                .ConfigureAwait(false);

            if (currentAttachment is not null)
            {
                string oldFullPath = Path.Join(basePath, currentAttachment.FilePath);

                if (File.Exists(oldFullPath))
                    File.Delete(oldFullPath);

                _ctx.Attachments.Remove(currentAttachment);
            }

            using (var stream = File.Create(fullPath))
            {
                await request.File.CopyToAsync(stream);
            }

            var entity = new Attachment
            {
                PresentationId = request.PresentationId,
                FilePath = randomFileName,
                FileName = request.File.FileName,
            };

            _ctx.Attachments.Add(entity);
            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Success();
        }
    }
}
