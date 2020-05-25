using BizBook.Common.Library.ApiExtensions;
using BizBook.Common.Library.Constants;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace BizBook.Common.Library.Attributes
{
    public class BizBookResponseAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            StringValues correlationId = context.HttpContext.Request.GetHeaderValue(HeaderNames.CorrelationId);
            context.HttpContext.Response.Headers.Add(Constants.HeaderNames.CorrelationId, correlationId);
            StringValues sessionId = context.HttpContext.Request.GetHeaderValue(HeaderNames.SessionId);
            context.HttpContext.Response.Headers.Add(Constants.HeaderNames.SessionId, sessionId);
            base.OnActionExecuted(context);
        }
    }
}