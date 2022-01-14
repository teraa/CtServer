using System.Linq.Expressions;
using CtServer.Data.Models;

namespace CtServer.Features.Presentations;

public record ReadModel
(
    int Id,
    int SectionId,
    string Title,
    string[] Authors,
    string Description,
    int Position,
    int DurationMinutes,
    string? MainAuthorPhoto
)
{
    public static Expression<Func<Presentation, ReadModel>> FromEntity
        => x => new ReadModel
        (
            x.Id,
            x.SectionId,
            x.Title,
            x.Authors,
            x.Description,
            x.Position,
            (int)x.Duration.TotalMinutes,
            x.MainAuthorPhoto
        );
}

public record WriteModel
(
    int SectionId,
    string Title,
    string[] Authors,
    string Description,
    int Position,
    int DurationMinutes,
    string? MainAuthorPhoto
);

public class WriteModelValidator : AbstractValidator<WriteModel>
{
    public WriteModelValidator()
    {
        RuleFor(x => x.SectionId).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Authors).NotEmpty().ForEach(x => x.NotEmpty());
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Position).GreaterThan(0);
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
    }
}
