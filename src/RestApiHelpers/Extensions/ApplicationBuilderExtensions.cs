using Microsoft.AspNetCore.Builder;
using RestApiHelpers.Logging;

namespace RestApiHelpers.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseSimpleRequestResponseLogging(this IApplicationBuilder builder, params string[] routesToIgnore)
        {
            SimpleRequestResponseLoggerMiddleware.SetRoutesToIgnore(routesToIgnore);

            builder.UseMiddleware<SimpleRequestResponseLoggerMiddleware>();
        }
    }
}