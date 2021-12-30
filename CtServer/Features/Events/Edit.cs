namespace CtServer.Features.Events;

public static class Edit
{
    public record Command
    (
        int Id,
        Model Model
    ) : IRequest<Response?>;

    public record Model
    (
        string Title,
        string Description,
        DateTimeOffset StartAt,
        DateTimeOffset EndAt
    );

    public class ModelValidator : AbstractValidator<Model>
    {
        public ModelValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.StartAt).NotEmpty();
            RuleFor(x => x.EndAt).NotEmpty();
        }
    }

    public record Response;

    public class Handler : IRequestHandler<Command, Response?>
    {
        private readonly CtDbContext _ctx;
        private readonly IMediator _mediator;

        public Handler(CtDbContext ctx, IMediator mediator)
        {
            _ctx = ctx;
            _mediator = mediator;
        }

        public async Task<Response?> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await _ctx.Events
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return null;

            entity.Title = request.Model.Title;
            entity.Description = request.Model.Description;
            entity.StartAt = request.Model.StartAt;
            entity.EndAt = request.Model.EndAt;

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _mediator.Publish(new Notifications.Edited.Notification(new(request.Id)))
                .ConfigureAwait(false);

            return new();
        }
    }
}
