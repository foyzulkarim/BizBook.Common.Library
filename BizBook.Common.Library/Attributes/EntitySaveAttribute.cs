using System;
using BizBook.Common.Library.ApiExtensions;
using BizBook.Common.Library.Constants;
using BizBook.Common.Library.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BizBook.Common.Library.Attributes
{
    public class EntitySaveAttribute : BizBookSecuredActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            base.OnActionExecuting(actionContext);

            string createdFrom = "Browser";
            bool containsCreatedFrom = actionContext.HttpContext.Request.Headers.ContainsKey(HeaderNames.CreatedFrom);
            if (containsCreatedFrom)
            {
                createdFrom = actionContext.HttpContext.Request.Headers[HeaderNames.CreatedFrom];
            }

            var data = actionContext.ActionArguments["model"];

            if (AppUser != null && data != null)
            {
                var isEntity = data is Entity;
                Entity entity = data as Entity;
                if (isEntity)
                {
                    entity.Id = Guid.NewGuid().ToString();
                    entity.Created = DateTime.UtcNow;
                    entity.Modified = DateTime.UtcNow;
                    entity.CreatedBy = AppUser.UserName;
                    entity.ModifiedBy = AppUser.UserName;
                    entity.IsActive = true;
                    entity.ShopId = AppUser.ShopId;
                    entity.CreatedFrom = createdFrom;
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