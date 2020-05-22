using System;
using System.Threading;
using BizBook.Common.Library.Models;
using BizBook.Common.Library.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BizBook.Common.Library.Attributes
{
    public class EntityEditFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var appUser = actionContext.HttpContext.Items["AppUser"] as BizBookUser;
            //bool tryGetValue = actionContext.ActionArguments.TryGetValue("AppUser", out appUser);
            object data = actionContext.ActionArguments["model"];
            bool tryGetValue = appUser != null;
            
            if (tryGetValue && data != null)
            {
                var user = appUser as BizBookUser;
                string username = user.UserName;

                var isEntity = data is Entity;
                Entity entity = data as Entity;
                if (isEntity)
                {
                    entity.Modified = DateTime.Now;
                    entity.ModifiedBy = username;
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