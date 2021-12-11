using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Sections;
    public static class Edit
    {
        public record Command
        (
            int Id,
            Model Model
        ) : IRequest<bool>;

        public record Model
        (
            int EventId,
            int LocationId,
            string Title,
            string[] Chairs,
            DateTimeOffset StartAt,
            DateTimeOffset EndAt,
            int BackgroundColor
        );

        public class ModelValidator : AbstractValidator<Model>
        {
            public ModelValidator()
            {
                RuleFor(x => x.EventId).GreaterThan(0);
                RuleFor(x => x.LocationId).GreaterThan(0);
                RuleFor(x => x.Title).NotEmpty();
                RuleFor(x => x.Chairs).NotEmpty().ForEach(x => x.NotEmpty());
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

                var entity = await ctx.Sections
                    .AsQueryable()
                    .Where(x => x.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);

                if (entity is null) return false;

                entity.EventId = request.Model.EventId;
                entity.LocationId = request.Model.LocationId;
                entity.Title = request.Model.Title;
                entity.Chairs = request.Model.Chairs;
                entity.StartAt = request.Model.StartAt;
                entity.EndAt = request.Model.EndAt;
                entity.BackgroundColor = request.Model.BackgroundColor;

                await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return true;
            }
        }
    }
