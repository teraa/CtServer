using CtServer.Data.Models;
using OneOf;

namespace CtServer.Features.Users;

public static class CreateSubscription
{
    public record Command
    (
        int UserId,
        Model Model
    ) : IRequest<OneOf<Success, NotFound>>;

    public record Model
    (
        string Endpoint,
        Model.KeysModel Keys
    )
    {
        public record KeysModel
        (
            string P256dh,
            string Auth
        );
    }

    public class ModelValidator : AbstractValidator<Model>
    {
        public ModelValidator()
        {
            RuleFor(x => x.Endpoint).NotEmpty();
            RuleFor(x => x.Keys).NotEmpty();
            RuleFor(x => x.Keys.P256dh).NotEmpty();
            RuleFor(x => x.Keys.Auth).NotEmpty();
        }
    }

    public record Success(int Id);
    public record NotFound;

    public class Handler : IRequestHandler<Command, OneOf<Success, NotFound>>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<OneOf<Success, NotFound>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _ctx.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                .ConfigureAwait(false);

            if (user is null) return new NotFound();

            var entity = new Subscription
            {
                UserId = user.Id,
                Endpoint = request.Model.Endpoint,
                P256dh = request.Model.Keys.P256dh,
                Auth = request.Model.Keys.Auth,
            };

            _ctx.Subscriptions.Add(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Success(entity.Id);
        }
    }
}
