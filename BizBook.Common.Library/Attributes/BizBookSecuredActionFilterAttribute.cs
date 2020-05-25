using BizBook.Common.Library.ApiExtensions;
using BizBook.Common.Library.Constants;
using BizBook.Common.Library.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BizBook.Common.Library.Attributes
{
    public abstract class BizBookSecuredActionFilterAttribute : ActionFilterAttribute
    {
        protected string CorrelationId;
        protected string SessionId;
        protected dynamic Controller;
        protected BizBookUser AppUser;
        protected TelemetryClient TelemetryClient;

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            CorrelationId = actionContext.HttpContext.Items[HeaderNames.CorrelationId].ToString();
            SessionId = actionContext.HttpContext.Items[HeaderNames.SessionId].ToString();
            AppUser = actionContext.HttpContext.Items["AppUser"] as BizBookUser;
            Controller = actionContext.Controller;
            Controller.AppUser = AppUser;

            TelemetryClient = Controller.TelemetryClient as TelemetryClient;
            TelemetryClient.SetGlobalProperties(SessionId, CorrelationId, AppUser, actionContext.ActionDescriptor.DisplayName);
        }
    }
}