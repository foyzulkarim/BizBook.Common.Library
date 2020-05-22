using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BizBook.Common.Library.ApiExtensions
{
    public static class LoggerExtension
    {
        public static IDisposable Initialize<T>(this ILogger<T> logger, string correlationId) where T : class
        {
            return logger.BeginScope("{CorrelationId} {UtcTime}", correlationId, DateTime.UtcNow);
        }

        public static IDisposable Initialize(this  ILogger logger, string correlationId)
        {
            return logger.BeginScope("{CorrelationId} {UtcTime}", correlationId, DateTime.UtcNow);
        }
    }
}
