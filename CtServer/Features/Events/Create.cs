using System;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using CtServer.Data.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Events
{
    public static class Create
    {
        public record Command
        (
            Model Model
        ) : IRequest<Result>;

        public record Model
        (
            string Title,
            DateTimeOffset StartAt,
            DateTimeOffset EndAt
        );

        public record Result(int Id);

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
                => _scopeFactory = scopeFactory;

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = new Event
                {
                    Title = request.Model.Title,
                    StartAt = request.Model.StartAt,
                    EndAt = request.Model.EndAt,
                };

                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

                ctx.Events.Add(entity);

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return new Result(entity.Id);
            }
        }
    }
}
