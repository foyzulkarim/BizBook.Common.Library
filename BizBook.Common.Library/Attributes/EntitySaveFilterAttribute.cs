using System;
using BizBook.Common.Library.Models;
using BizBook.Common.Library.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BizBook.Common.Library.Attributes
{
    public class EntitySaveFilterAttribute : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var data = actionContext.ActionArguments["model"];
            var appUser = actionContext.HttpContext.Items["AppUser"] as BizBookUser;
            string createdFrom = "Browser";
            bool containsCreatedFrom = actionContext.HttpContext.Request.Headers.ContainsKey("x-bizbook-created-from");
            if (containsCreatedFrom)
            {
                createdFrom = actionContext.HttpContext.Request.Headers["x-bizbook-created-from"];
            }

            bool tryGetValue = appUser != null;
            if (tryGetValue && data != null)
            {
                string username = appUser.UserName;
                var isEntity = data is Entity;
                Entity entity = data as Entity;
                if (isEntity)
                {
                    entity.Id = Guid.NewGuid().ToString();
                    entity.Created = DateTime.UtcNow;
                    entity.Modified = DateTime.UtcNow;
                    entity.CreatedBy = username;
                    entity.ModifiedBy = username;
                    entity.IsActive = true;
                    entity.ShopId = appUser.ShopId;
                    entity.CreatedFrom = createdFrom;
                }

                dynamic controller = actionContext.Controller;
                controller.AppUser = appUser;
            }
            else
            {
                actionContext.Result = new BadRequestResult();
            }
        }
    }
}