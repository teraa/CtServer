using CtServer.Options;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using OneOf;

namespace CtServer.Features.Presentations.Photos;

public static class Get
{
    public record Query(int PresentationId) : IRequest<OneOf<Success, NotFound>>;

    public record Success
    (
        string FilePath,
        string FileName,
        string ContentType
    );

    public class Handler : IRequestHandler<Query, OneOf<Success, NotFound>>
    {
        private readonly CtDbContext _ctx;
        private readonly StorageOptions _storageOptions;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public Handler(
            CtDbContext ctx,
            IOptions<StorageOptions> storageOptions,
            FileExtensionContentTypeProvider contentTypeProvider)
        {
            _ctx = ctx;
            _storageOptions = storageOptions.Value;
            _contentTypeProvider = contentTypeProvider;
        }

        public async Task<OneOf<Success, NotFound>> Handle(Query request, CancellationToken cancellationToken)
        {
            string basePath = Path.GetFullPath(_storageOptions.PhotosPath);

            var entity = await _ctx.Photos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PresentationId == request.PresentationId, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return new NotFound();

            if (!_contentTypeProvider.TryGetContentType(entity.FileName, out string? contentType))
                contentType = "application/octet-stream";

            var model = new Success
            (
                FilePath: Path.Join(basePath, entity.FilePath),
                FileName: entity.FileName,
                ContentType: contentType
            );

            return model;
        }
    }
}
