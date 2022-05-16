using CtServer.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OneOf;

namespace CtServer.ActionFilters;

public class ResultFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is not ObjectResult { Value: IOneOf union })
            return;

        context.Result = union.Value switch
        {
            Success _ => new NoContentResult(),
            NotFound _ => new NotFoundResult(),
            Fail x => new BadRequestObjectResult(x),
            object x => new OkObjectResult(x)
        };
    }
}
