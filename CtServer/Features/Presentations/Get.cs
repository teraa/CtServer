namespace CtServer.Features.Presentations;

public static class Get
{
    public record Query
    (
        int Id
    ) : IRequest<ReadModel?>;

    public class Handler : IRequestHandler<Query, ReadModel?>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<ReadModel?> Handle(Query request, CancellationToken cancellationToken)
        {
            var model = await _ctx.Presentations
                .AsNoTracking()
                .Where(x => x.Id == request.Id)
                .Select(ReadModel.FromEntity)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return model;
        }
    }
}
