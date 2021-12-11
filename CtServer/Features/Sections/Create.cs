using System;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using CtServer.Data.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Sections;
public static class Create
{
    public record Command
    (
        Model Model
    ) : IRequest<Result>;

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
                LocationId = request.Model.LocationId,
                Title = request.Model.Title,
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
