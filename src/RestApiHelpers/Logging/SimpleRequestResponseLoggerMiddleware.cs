using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace RestApiHelpers.Logging
{
    public class SimpleRequestResponseLoggerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<SimpleRequestResponseLoggerMiddleware> logger;

        private static string[] routesToIgnore;

        public SimpleRequestResponseLoggerMiddleware(RequestDelegate next, ILogger<SimpleRequestResponseLoggerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public static void SetRoutesToIgnore(params string[] routesToIgnore)
        {
            SimpleRequestResponseLoggerMiddleware.routesToIgnore = routesToIgnore;
        }

        public async Task Invoke(HttpContext context)
        {
            var doLogging = routesToIgnore == null || !routesToIgnore.Any(r => context.Request.Path.StartsWithSegments(r));

            if (doLogging)
            {
                logger.LogInformation($"Request starting {context.Request.Method} {context.Request.GetDisplayUrl()}");
            }

            var stopwatch = Stopwatch.StartNew();

            await next.Invoke(context);

            if (doLogging)
            {
                stopwatch.Stop();

                logger.LogInformation($"Request finished in {stopwatch.ElapsedMilliseconds}ms {context.Response.StatusCode} {context.Response.ContentType}");
            }
        }
    }
}