using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Presentations
{
    public static class Edit
    {
        public record Command
        (
            int Id,
            Model Model
        ) : IRequest<bool>;
        public record Model
        (
            int SectionId,
            string Title,
            string[] Authors,
            string Description,
            DateTimeOffset StartAt,
            DateTimeOffset EndAt,
            string? Attachment,
            string? MainAuthorPhoto
        );

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
                => _scopeFactory = scopeFactory;

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

                var entity = await ctx.Presentations
                    .AsQueryable()
                    .Where(x => x.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (entity is null) return false;

                entity.SectionId = request.Model.SectionId;
                entity.Title = request.Model.Title;
                entity.Authors = request.Model.Authors;
                entity.Description = request.Model.Description;
                entity.StartAt = request.Model.StartAt;
                entity.EndAt = request.Model.EndAt;
                entity.Attachment = request.Model.Attachment;
                entity.MainAuthorPhoto = request.Model.MainAuthorPhoto;

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return true;
            }
        }
    }
}
