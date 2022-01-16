using System.Linq.Expressions;
using CtServer.Data.Models;

namespace CtServer.Features.Events;

public record ReadModel
(
    int Id,
    string Title,
    string Description,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt
)
{
    public static Expression<Func<Event, ReadModel>> FromEntity
        => x => new ReadModel
        (
            x.Id,
            x.Title,
            x.Description,
            x.StartAt,
            x.EndAt
        );
}

public record WriteModel
(
    string Title,
    string Description,
    DateTimeOffset StartAt,
    DateTimeOffset EndAt
);

public class WriteModelValidator : AbstractValidator<WriteModel>
{
    public WriteModelValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotNull();
        RuleFor(x => x.StartAt).NotEmpty();
        RuleFor(x => x.EndAt).NotEmpty().GreaterThan(x => x.StartAt);
    }
}
