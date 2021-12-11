using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using CtServer.Data.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Locations;
    public static class Create
    {
        public record Command
        (
            Model Model
        ) : IRequest<Result>;

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

        public record Result(int Id);

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
                => _scopeFactory = scopeFactory;

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var entity = new Location
                {
                    EventId = request.Model.EventId,
                    Name = request.Model.Name,
                };

                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

                ctx.Locations.Add(entity);

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return new Result(entity.Id);
            }
        }
    }
