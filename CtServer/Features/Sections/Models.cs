using System.Linq.Expressions;
using CtServer.Data.Models;

namespace CtServer.Features.Sections;

public record ReadModel
(
    int Id,
    int EventId,
    int LocationId,
    string Title,
    string[] Chairs,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    int BackgroundColor
)
{
    public static Expression<Func<Section, ReadModel>> FromEntity
        => x => new ReadModel
        (
            x.Id,
            x.EventId,
            x.LocationId,
            x.Title,
            x.Chairs,
            x.StartAt,
            x.EndAt,
            x.BackgroundColor
        );
}

public record WriteModel
(
    int EventId,
    int LocationId,
    string Title,
    string[] Chairs,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt,
    int BackgroundColor
);

public class WriteModelValidator : AbstractValidator<WriteModel>
{
    public WriteModelValidator()
    {
        RuleFor(x => x.EventId).GreaterThan(0);
        RuleFor(x => x.LocationId).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Chairs).NotEmpty()
            .ForEach(x => x.NotEmpty());
        RuleFor(x => x.StartAt).NotEmpty();
        RuleFor(x => x.EndAt).NotEmpty()
            .GreaterThan(x => x.StartAt);
        RuleFor(x => x.BackgroundColor)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(0xFFFFFF);
    }
}
