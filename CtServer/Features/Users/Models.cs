using System.Linq.Expressions;
using CtServer.Data.Models;

namespace CtServer.Features.Users;

public record ReadModel
(
    int Id,
    string Username
)
{
    public static Expression<Func<User, ReadModel>> FromEntity
        => x => new ReadModel
        (
            x.Id,
            x.Username
        );
}
