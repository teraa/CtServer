using System.Threading;
using System.Threading.Tasks;
using CtServer.Data;
using CtServer.Data.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CtServer.Features.Presentations
{
    public static class Create
    {
        public record Command
        (
            Model Model
        ) : IRequest<Result>;

        public record Model
        (
            int SectionId,
            string Title,
            string[] Authors,
            string Description,
            int Position,
            string? Attachment,
            string? MainAuthorPhoto
        );

        public class ModelValidator : AbstractValidator<Model>
        {
            public ModelValidator()
            {
                RuleFor(x => x.SectionId).GreaterThan(0);
                RuleFor(x => x.Title).NotEmpty();
                RuleFor(x => x.Authors).NotEmpty().ForEach(x => x.NotEmpty());
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.Position).GreaterThan(0);
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
                var entity = new Presentation
                {
                    SectionId = request.Model.SectionId,
                    Title = request.Model.Title,
                    Authors = request.Model.Authors,
                    Description = request.Model.Description,
                    Position = request.Model.Position,
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
