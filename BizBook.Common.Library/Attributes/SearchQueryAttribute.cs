using System;
using System.IO;
using System.Threading.Tasks;
using BizBook.Common.Library.ApiExtensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace BizBook.Common.Library.Attributes
{
    public class SearchQueryAttribute : BizBookSecuredActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            Stream requestBody = actionContext.HttpContext.Request.Body;
            var reader = new StreamReader(requestBody);
            var readToAsyncTask = Task.Run(() => reader.ReadToEndAsync());
            string result = readToAsyncTask.Result;
            var actionDescriptorParameters = actionContext.ActionDescriptor.Parameters;

            if (actionDescriptorParameters.Count > 0)
            {
                Type type = Type.GetType(actionDescriptorParameters[0].ParameterType.AssemblyQualifiedName ?? string.Empty);
                if (type != null)
                {
                    var deserializeObject = JsonConvert.DeserializeObject(result, type) ?? new System.Dynamic.ExpandoObject();
                    actionContext.ActionArguments.Add("request", deserializeObject);

                    var data = new
                    {
                        Type = type.Name, Request = result
                    };
                
                    this.TelemetryClient.TrackObject(data);
                }
            }
            else
            {
                actionContext.ActionArguments.Add("request", new System.Dynamic.ExpandoObject());
            }
        }
    }
}