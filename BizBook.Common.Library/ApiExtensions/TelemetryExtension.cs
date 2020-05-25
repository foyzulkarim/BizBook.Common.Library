using System;
using System.Collections.Generic;
using BizBook.Common.Library.Constants;
using BizBook.Common.Library.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BizBook.Common.Library.ApiExtensions
{
    public static class TelemetryExtension
    {
        public static IDisposable Initialize<T>(this ILogger<T> logger, string correlationId) where T : class
        {
            return logger.BeginScope("{CorrelationId} {UtcTime}", correlationId, DateTime.UtcNow);
        }

        public static IDisposable Initialize(this  ILogger logger, string correlationId)
        {
            return logger.BeginScope("{CorrelationId} {UtcTime}", correlationId, DateTime.UtcNow);
        }

        public static void SetGlobalProperties(this TelemetryClient telemetry, string sessionId, string correlationId, BizBookUser appUser, string method)
        {
            telemetry.Context.GlobalProperties[HeaderNames.SessionId] = sessionId;
            telemetry.Context.GlobalProperties[HeaderNames.CorrelationId] = correlationId;
            telemetry.Context.GlobalProperties[HeaderNames.UserName] = appUser.UserName;
            telemetry.Context.GlobalProperties[HeaderNames.ShopId] = appUser.ShopId;
            telemetry.Context.GlobalProperties[HeaderNames.RequestMethod] = method;
        }

        public static void TrackObject(this TelemetryClient telemetryClient, object data, IDictionary<string, string> properties = null, SeverityLevel severityLevel = SeverityLevel.Information)
        {
            var serializeObject = JsonConvert.SerializeObject(data);
            properties ??= new Dictionary<string, string>();
            telemetryClient.TrackTrace($"Model: {data} Content: {serializeObject}", severityLevel, properties);
        }

        public static void TrackExceptionWithCorrelationId(this TelemetryClient telemetryClient, Exception exception, string correlationId)
        {
            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { HeaderNames.CorrelationId, correlationId }
            };

            telemetryClient.TrackException(exception, properties);
        }
    }
}
