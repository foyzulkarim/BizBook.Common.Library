using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace BizBook.Common.Library.ApiExtensions
{
    public static class HttpRequestExtensions
    {
        public static string GetHeaderValue(this HttpRequest request, string headerName)
        {
            StringValues source;
            if (!request.Headers.TryGetValue(headerName, out source))
                return Guid.NewGuid().ToString();
            string str = source.FirstOrDefault<string>();
            return string.IsNullOrWhiteSpace(str) ? Guid.NewGuid().ToString() : str;
        }
    }
}
