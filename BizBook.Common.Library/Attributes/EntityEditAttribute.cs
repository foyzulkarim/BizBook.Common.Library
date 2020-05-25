using System;
using BizBook.Common.Library.ApiExtensions;
using BizBook.Common.Library.Models;
using BizBook.Common.Library.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BizBook.Common.Library.Attributes
{
    public class EntityEditAttribute : BizBookSecuredActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            object data = actionContext.ActionArguments["model"];

            if (AppUser != null && data != null)
            {
                var isEntity = data is Entity;
                Entity entity = data as Entity;
                if (isEntity)
                {
                    entity.Modified = DateTime.UtcNow;
                    entity.ModifiedBy = AppUser.UserName;
                }

                this.TelemetryClient.TrackObject(entity);
            }
            else
            {
                actionContext.Result = new BadRequestResult();
            }
        }
    }
}