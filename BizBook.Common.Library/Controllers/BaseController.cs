using System;
using System.Threading.Tasks;
using BizBook.Common.Library.ApiExtensions;
using BizBook.Common.Library.Attributes;
using BizBook.Common.Library.Constants;
using BizBook.Common.Library.Models;
using BizBook.Common.Library.Models.Entities;
using BizBook.Common.Library.Models.RequestModels;
using BizBook.Common.Library.Models.ViewModels;
using BizBook.Common.Library.Services;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BizBook.Common.Library.Controllers
{
    [BizBookAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<T, TR, TV, TV2> : ControllerBase where T : Entity where TR : RequestModel<T> where TV : BaseViewModel<T> where TV2: BaseBasicViewModel<T>
    {
        protected IBaseService<T, TR, TV, TV2> Service;
        protected string Type;

        public BizBookUser AppUser { get; set; }

        public TelemetryClient TelemetryClient { get; }

        protected BaseController(IBaseService<T, TR, TV, TV2> service, string type, TelemetryClient telemetryClient)
        {
            // this.Logger = logger;
            this.Service = service;
            this.Type = type;
            TelemetryClient = telemetryClient;
        }

        [EntitySave]
        [BizBookResponse]
        [Route("Add")]
        [HttpPost]
        public virtual async Task<ActionResult> Add([FromBody] T model)
        {
            var correlationId = this.Request.GetHeaderValue(HeaderNames.CorrelationId);
            var data = model;
            if (!ModelState.IsValid)
            {
                this.TelemetryClient.TrackTrace($"User sent Invalid model {model.GetType()}", SeverityLevel.Warning);
                this.TelemetryClient.TrackObject(ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var add = await this.Service.AddAsync(model);
                this.TelemetryClient.TrackTrace($"Model {data} saved Id {data.Id}");
                return Ok(model.Id);
            }
            catch (Exception exception)
            {
                this.TelemetryClient.TrackException(exception);
                string message = $"CorrelationId: {correlationId}, Error: {exception.Message}";
                var result = StatusCode(StatusCodes.Status500InternalServerError, message);
                return result;
            }
        }

        [SearchQuery]
        [BizBookResponse]
        [Route("Search")]
        [HttpPost]
        public virtual async Task<IActionResult> Search([FromBody] TR request)
        {
            try
            {
                var content = await this.Service.SearchAsync(request);
                return Ok(content);
            }
            catch (Exception exception)
            {
                string correlationId = this.Request.GetHeaderValue(HeaderNames.CorrelationId);
                this.TelemetryClient.TrackExceptionWithCorrelationId(exception, correlationId);
                string message = $"CorrelationId: {correlationId}, Error: {exception.Message}";
                return StatusCode(500, message);
            }
        }

        [SearchQuery]
        [BizBookResponse]
        [Route("BasicSearch")]
        [HttpPost]
        public virtual async Task<IActionResult> BasicSearch([FromBody] TR request)
        {
            try
            {
                var content = await this.Service.BasicSearchAsync(request);
                return Ok(content);
            }
            catch (Exception exception)
            {
                string correlationId = this.Request.GetHeaderValue(HeaderNames.CorrelationId);
                this.TelemetryClient.TrackExceptionWithCorrelationId(exception, correlationId);
                string message = $"CorrelationId: {correlationId}, Error: {exception.Message}";
                return StatusCode(500, message);
            }
        }
    }
}
