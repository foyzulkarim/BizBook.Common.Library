using System;
using System.Linq;
using System.Security.Claims;
using BizBook.Common.Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BizBook.Common.Library.Attributes
{
    public class BizBookAuthorizationAttribute : AuthorizeAttribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                ClaimsPrincipal user = context.HttpContext.User;
                string id = user.Claims.First(x => x.Type == JwtClaimIdentifiers.Id).Value;
                var shopId = user.Claims.First(x => x.Type == JwtClaimIdentifiers.ShopId).Value;
                var userName = user.Claims.First(x => x.Type == JwtClaimIdentifiers.UserName).Value;

                BizBookUser appUser = new BizBookUser()
                {
                    Id = id,
                    ShopId = shopId,
                    UserName = userName
                };

                context.HttpContext.Items["AppUser"] = appUser;
            }
            else
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
        }

        //protected override bool IsAuthorized(HttpActionContext actionContext)
        //{
        //    //return true;
        //    string bizbookserveridentity = ConfigurationManager<>.AppSettings["IdentityServer"];
        //    var request = actionContext.Request;
        //    string resourceName = request.RequestUri.AbsolutePath;
        //    var last = resourceName.Split(new string[] { "/api" }, StringSplitOptions.RemoveEmptyEntries).Last();
        //    resourceName = "/api" + last;
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization = request.Headers.Authorization;
        //        PermissionRequest permissionRequest = new PermissionRequest { Name = resourceName };
        //        string url = bizbookserveridentity + "api/Authorization/Authorize";
        //        var responseMessage = client.PostAsJsonAsync(url, permissionRequest).Result;
        //        string result = responseMessage.Content.ReadAsStringAsync().Result;

        //        bool isAuthorized = responseMessage.StatusCode == HttpStatusCode.OK;
        //        if (isAuthorized)
        //        {
        //            BizBookUser user = JsonConvert.DeserializeObject<BizBookUser>(result);
        //            if (user != null)
        //            {
        //                actionContext.Request.Properties.Add("AppUser", user);
        //                dynamic controller = actionContext.ControllerContext.Controller;
        //                controller.AppUser = user;
        //            }
        //        }

        //        return isAuthorized;
        //    }
        //}

    }
}