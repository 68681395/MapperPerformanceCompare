using System;
using System.Net;

namespace PerformanceTool.SharpHttpServer
{
    public static class HttpListenerRequestExtensions
    {
        public static HttpMethod GetHttpMethod(this HttpListenerRequest request)
        {
            return (HttpMethod)Enum.Parse(typeof(HttpMethod), request.HttpMethod, ignoreCase: true);
        }
    }
}
