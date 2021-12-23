using CtServer.Data.Models;

namespace CtServer.Features.Presentations;

public static class Create
{
    public record Command
    (
        Model Model
    ) : IRequest<Response>;

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

    public record Response(int Id);

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly CtDbContext _ctx;

        public Handler(CtDbContext ctx)
            => _ctx = ctx;

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = new Presentation
            {
                SectionId = request.Model.SectionId,
                Title = request.Model.Title,
                Authors = request.Model.Authors,
                Description = request.Model.Description,
                Position = request.Model.Position,
                Duration = TimeSpan.FromMinutes(request.Model.DurationMinutes),
                Attachment = request.Model.Attachment,
                MainAuthorPhoto = request.Model.MainAuthorPhoto,
            };

            _ctx.Presentations.Add(entity);

            await _ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new(entity.Id);
        }
    }
}
