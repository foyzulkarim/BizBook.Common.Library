using System;
using System.Threading.Tasks;
using BizBook.Common.Library.ApiExtensions;
using BizBook.Common.Library.Attributes;
using BizBook.Common.Library.Models;
using BizBook.Common.Library.Models.Entities;
using BizBook.Common.Library.Models.RequestModels;
using BizBook.Common.Library.Models.ViewModels;
using BizBook.Common.Library.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BizBook.Common.Library.Controllers
{
    [BizBookAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<T, TR, TV> : ControllerBase where T : Entity where TR : RequestModel<T> where TV : BaseViewModel<T>
    {
        protected IBaseService<T, TR, TV> Service;
        protected ILogger Logger;
        protected string Type;

        public BizBookUser AppUser { get; set; }

        protected BaseController(IBaseService<T, TR, TV> service, ILogger logger, string type)
        {
            this.Logger = logger;
            this.Service = service;
            this.Type = type;
        }

        [EntitySaveFilter]
        [BizBookResponse]
        [Route("Add")]
        [HttpPost]
        public virtual async Task<ActionResult> Add([FromBody] T model)
        {
            var correlationId = this.Request.GetCorrelationId();
            var scopedLogger = this.Logger.Initialize(correlationId);
            using (scopedLogger)
            {
                var data = model;
                if (!ModelState.IsValid)
                {
                    this.Logger.LogWarning("User sent Invalid model {type} state {Data}", model.GetType().ToString(), data);
                    return BadRequest(ModelState);
                }

                try
                {
                    var add = await this.Service.AddAsync(model);
                    this.Logger.LogDebug("User {UserName} Added entity {TypeName} {Id}", AppUser.UserName, this.Type, data.Id);
                    return Ok(model.Id);
                }
                catch (Exception exception)
                {
                    this.Logger.LogError(
                        exception,
                        "Exception occurred while saving {Data} by User {AppUser}",
                        data, AppUser.UserName);
                    var result = StatusCode(StatusCodes.Status500InternalServerError, exception);
                    return result;
                }
            }
           
        }

        [SearchQuery]
        [BizBookResponse]
        [Route("Search")]
        [HttpPost]
        public virtual async Task<IActionResult> Search([FromBody] TR request)
        {
            var correlationId = this.Request.GetCorrelationId();
            var scopedLogger = this.Logger.Initialize(correlationId);
            using (scopedLogger)
            {
                try
                {
                    var content = await this.Service.SearchAsync(request);
                    return Ok(content);
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, "Exception occurred while Searching {TypeName} with Request {Request}", Type, request);
                    return StatusCode(500, exception.Message);
                }
            }
        }
    }
}
