using System;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using CtServer.Data.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Sections
{
    public static class Create
    {
        public record Command
        (
            Model Model
        ) : IRequest<Result>;

        public record Model
        (
            int EventId,
            string Title,
            string Location,
            string[] Chairs,
            DateTimeOffset StartAt,
            DateTimeOffset EndAt,
            int BackgroundColor
        );

        public record Result(int Id);

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
                => _scopeFactory = scopeFactory;

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = new Section
                {
                    EventId = request.Model.EventId,
                    Title = request.Model.Title,
                    Location = request.Model.Location,
                    Chairs = request.Model.Chairs,
                    StartAt = request.Model.StartAt,
                    EndAt = request.Model.EndAt,
                    BackgroundColor = request.Model.BackgroundColor,
                };

                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

                ctx.Sections.Add(entity);

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return new Result(entity.Id);
            }
        }
    }
}
