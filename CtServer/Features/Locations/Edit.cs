using CtServer.Results;
using OneOf;

namespace CtServer.Features.Locations;

public static class Edit
{
    public record Command
    (
        int Id,
        Model Model
    ) : IRequest<OneOf<Success, NotFound>>;

    public record Model
    (
        int EventId,
        string Name
    );

    public class ModelValidator : AbstractValidator<Model>
    {
        public ModelValidator()
        {
            RuleFor(x => x.EventId).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty();
        }
    }


    public class Handler : IRequestHandler<Command, OneOf<Success, NotFound>>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<OneOf<Success, NotFound>> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.Locations
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return new NotFound();

            entity.EventId = request.Model.EventId;
            entity.Name = request.Model.Name;

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new Success();
        }
    }
}
