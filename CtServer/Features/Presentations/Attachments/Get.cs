using OneOf;

namespace CtServer.Features.Presentations.Attachments;

public static class Get
{
    public record Query(int PresentationId) : IRequest<OneOf<Success, NotFound>>;

    public record Success
    (
        string FilePath,
        string FileName
    );

    public class Handler : IRequestHandler<Query, OneOf<Success, NotFound>>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<OneOf<Success, NotFound>> Handle(Query request, CancellationToken cancellationToken)
        {
            var model = await _ctx.Attachments
                .AsNoTracking()
                .Where(x => x.PresentationId == request.PresentationId)
                .Select(x => new Success
                (
                    x.FilePath,
                    x.FileName
                ))
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (model is null) return new NotFound();

            return model;
        }
    }
}
