namespace CtServer.Features.Users;

public static class Index
{
    public record Query : IRequest<ReadModel[]>;

    public class Handler : IRequestHandler<Query, ReadModel[]>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<ReadModel[]> Handle(Query request, CancellationToken cancellationToken)
        {
            var models = await _ctx.Users
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(ReadModel.FromEntity)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            return models;
        }
    }
}
