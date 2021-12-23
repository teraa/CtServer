namespace CtServer.Features.Users;

public static class Get
{
    public record Query(int Id) : IRequest<Model?>;

    public record Model
    (
        int Id,
        string Username
    );

    public class Handler : IRequestHandler<Query, Model?>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Model?> Handle(Query request, CancellationToken cancellationToken)
        {
            var model = await _ctx.Users
                .AsNoTracking()
                .AsSingleQuery()
                .Where(x => x.Id == request.Id)
                .Select(x => new Model
                (
                    x.Id,
                    x.Username
                ))
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return model;
        }
    }
}