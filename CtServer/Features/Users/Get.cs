using OneOf;

namespace CtServer.Features.Users;

public static class Get
{
    public record Query(int Id) : IRequest<OneOf<ReadModel, NotFound>>;

    public class Handler : IRequestHandler<Query, OneOf<ReadModel, NotFound>>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<OneOf<ReadModel, NotFound>> Handle(Query request, CancellationToken cancellationToken)
        {
            var model = await _ctx.Users
                .AsNoTracking()
                .AsSingleQuery()
                .Where(x => x.Id == request.Id)
                .Select(ReadModel.FromEntity)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (model is null)
                return new NotFound();

            return model;
        }
    }
}
