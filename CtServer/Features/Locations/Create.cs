using CtServer.Data.Models;

namespace CtServer.Features.Locations;

public static class Create
{
    public record Command
    (
        Model Model
    ) : IRequest<Response>;

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

    public record Response(int Id);

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = new Location
            {
                EventId = request.Model.EventId,
                Name = request.Model.Name,
            };

            _ctx.Locations.Add(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new(entity.Id);
        }
    }
}
