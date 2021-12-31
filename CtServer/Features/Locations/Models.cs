using System.Linq.Expressions;
using CtServer.Data.Models;

namespace CtServer.Features.Locations;

public record ReadModel
(
    int Id,
    int EventId,
    string Name
)
{
    public static Expression<Func<Location, ReadModel>> FromEntity
        => x => new ReadModel
        (
            x.Id,
            x.EventId,
            x.Name
        );
}

public record WriteModel
(
    int EventId,
    string Name
);

public class WriteModelValidator : AbstractValidator<WriteModel>
{
    public WriteModelValidator()
    {
        RuleFor(x => x.EventId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
    }
}
