using System;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Events;
    public static class Edit
    {
        public record Command
        (
            int Id,
            Model Model
        ) : IRequest<bool>;

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

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly IServiceScopeFactory _scopeFactory;

            public Handler(IServiceScopeFactory scopeFactory)
                => _scopeFactory = scopeFactory;

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                using var scope = _scopeFactory.CreateScope();
                var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

                var entity = await ctx.Events
                    .AsQueryable()
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (entity is null) return false;

                entity.Title = request.Model.Title;
                entity.Description = request.Model.Description;
                entity.StartAt = request.Model.StartAt;
                entity.EndAt = request.Model.EndAt;

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return true;
            }
        }
    }
