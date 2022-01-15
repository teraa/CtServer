using Microsoft.AspNetCore.Authorization;

namespace CtServer.Authorization;

public class AdminRequirement : IAuthorizationRequirement
{ }

public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
{
    private readonly CtDbContext _dbContext;

    public AdminAuthorizationHandler(CtDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
    {
        if (!int.TryParse(context.User.GetUserId(), out int id))
        {
            context.Fail();
            return;
        }

        bool isAdmin = await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => x.IsAdmin)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (isAdmin)
            context.Succeed(requirement);
        else
            context.Fail();
    }
}
