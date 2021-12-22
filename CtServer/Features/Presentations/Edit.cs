namespace CtServer.Features.Presentations;

public static class Edit
{
    public record Command
    (
        int Id,
        Model Model
    ) : IRequest<Response?>;

    public record Model
    (
        int SectionId,
        string Title,
        string[] Authors,
        string Description,
        int Position,
        int DurationMinutes,
        string? Attachment,
        string? MainAuthorPhoto
    );

    public class ModelValidator : AbstractValidator<Model>
    {
        public ModelValidator()
        {
            RuleFor(x => x.SectionId).GreaterThan(0);
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Authors).NotEmpty().ForEach(x => x.NotEmpty());
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Position).GreaterThan(0);
            RuleFor(x => x.DurationMinutes).GreaterThan(0);
        }
    }

    public record Response;

    public class Handler : IRequestHandler<Command, Response?>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public Handler(IServiceScopeFactory scopeFactory)
            => _scopeFactory = scopeFactory;

        public async Task<Response?> Handle(Command request, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

            var entity = await ctx.Presentations
                .AsQueryable()
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (entity is null) return null;

            entity.SectionId = request.Model.SectionId;
            entity.Title = request.Model.Title;
            entity.Authors = request.Model.Authors;
            entity.Description = request.Model.Description;
            entity.Position = request.Model.Position;
            entity.Duration = TimeSpan.FromMinutes(request.Model.DurationMinutes);
            entity.Attachment = request.Model.Attachment;
            entity.MainAuthorPhoto = request.Model.MainAuthorPhoto;

            await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new();
        }
    }
}
