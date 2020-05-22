using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace BizBook.Common.Library.ApiExtensions
{
    public static class HttpRequestExtensions
    {
        public static string GetCorrelationId(this HttpRequest request)
        {
            StringValues source;
            if (!request.Headers.TryGetValue("x-bizbook-correlation-id", out source))
                return Guid.NewGuid().ToString();
            string str = source.FirstOrDefault<string>();
            return string.IsNullOrWhiteSpace(str) ? Guid.NewGuid().ToString() : str;
        }
    }
}
