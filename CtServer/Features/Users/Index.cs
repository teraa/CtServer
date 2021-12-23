namespace CtServer.Features.Users;

public static class Index
{
    public record Query : IRequest<Model[]>;

    public record Model
    (
        int Id,
        string Username
    );

    public class Handler : IRequestHandler<Query, Model[]>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Model[]> Handle(Query request, CancellationToken cancellationToken)
        {
            var models = await _ctx.Users
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new Model
                (
                    x.Id,
                    x.Username
                ))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            return models;
        }
    }
}
