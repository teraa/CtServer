using CtServer.Data.Models;

namespace CtServer.Features.Presentations;

public static class Create
{
    public record Command
    (
        WriteModel Model
    ) : IRequest<Response>;

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
