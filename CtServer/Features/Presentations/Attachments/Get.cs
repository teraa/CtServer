using CtServer.Options;
using Microsoft.Extensions.Options;
using OneOf;

namespace CtServer.Features.Presentations.Attachments;

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

        public Handler(CtDbContext ctx, IOptions<StorageOptions> storageOptions)
        {
            _ctx = ctx;
            _storageOptions = storageOptions.Value;
        }

        public async Task<OneOf<Success, NotFound>> Handle(Query request, CancellationToken cancellationToken)
        {
            string basePath = Path.GetFullPath(_storageOptions.AttachmentsPath);

            var model = await _ctx.Attachments
                .AsNoTracking()
                .Where(x => x.PresentationId == request.PresentationId)
                .Select(x => new Success
                (
                    Path.Join(basePath, x.FilePath),
                    x.FileName,
                    "application/octet-stream"
                ))
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (model is null) return new NotFound();

            return model;
        }
    }
}
