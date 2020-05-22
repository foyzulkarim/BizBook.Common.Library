using BizBook.Common.Library.ApiExtensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace BizBook.Common.Library.Attributes
{
    public class BizBookResponseAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            StringValues correlationId = context.HttpContext.Request.GetCorrelationId();
            context.HttpContext.Response.Headers.Add(Constants.HeaderNames.CorrelationId, correlationId);
            base.OnActionExecuted(context);
        }
    }
}