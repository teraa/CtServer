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
            var filePath = Path.Combine(_storageOptions.AttachmentsPath, Path.GetRandomFileName());

            var currentAttachment = await _ctx.Attachments
                .FirstOrDefaultAsync(x => x.PresentationId == request.PresentationId, cancellationToken)
                .ConfigureAwait(false);

            if (currentAttachment is not null)
            {
                if (File.Exists(currentAttachment.FilePath))
                    File.Delete(currentAttachment.FilePath);

                _ctx.Attachments.Remove(currentAttachment);
            }

            using (var stream = File.Create(filePath))
            {
                await request.File.CopyToAsync(stream);
            }

            var entity = new Attachment
            {
                PresentationId = request.PresentationId,
                FilePath = filePath,
                FileName = request.File.FileName,
            };

            _ctx.Attachments.Add(entity);
            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Success();
        }
    }
}
