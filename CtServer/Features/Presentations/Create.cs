using System;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using CtServer.Data.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Presentations
{
    public static class Create
    {
        public record Command
        (
            int EventId,
            int SectionId,
            Model Model
        ) : IRequest<Result>;

        public record Model
        (
            string Title,
            string[] Authors,
            string Description,
            DateTimeOffset StartAt,
            DateTimeOffset EndAt,
            string? Attachment,
            string? MainAuthorPhoto
        );

        public record Result(int Id);

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
                => _scopeFactory = scopeFactory;

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = new Presentation
                {
                    SectionId = request.SectionId,
                    Title = request.Model.Title,
                    Authors = request.Model.Authors,
                    Description = request.Model.Description,
                    StartAt = request.Model.StartAt,
                    EndAt = request.Model.EndAt,
                    Attachment = request.Model.Attachment,
                    MainAuthorPhoto = request.Model.MainAuthorPhoto,
                };

                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

                ctx.Presentations.Add(entity);

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return new Result(entity.Id);
            }
        }
    }
}
