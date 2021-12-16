using System;
using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using CtServer.Data.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Events;

public static class Create
{
    public record Command
    (
        Model Model
    ) : IRequest<Response>;

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

    public record Response(int Id);

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public Handler(IServiceScopeFactory scopeFactory)
            => _scopeFactory = scopeFactory;

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = new Event
            {
                Title = request.Model.Title,
                Description = request.Model.Description,
                StartAt = request.Model.StartAt,
                EndAt = request.Model.EndAt,
            };

            using var scope = _scopeFactory.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<CtDbContext>();

            ctx.Events.Add(entity);

            await ctx.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new(entity.Id);
        }
    }
}
