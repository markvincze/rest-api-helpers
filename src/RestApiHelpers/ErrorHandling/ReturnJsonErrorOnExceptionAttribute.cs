using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace RestApiHelpers.ErrorHandling
{
    public class ReturnJsonErrorOnExceptionAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ReturnJsonErrorOnExceptionAttribute> logger;

        public ReturnJsonErrorOnExceptionAttribute(ILogger<ReturnJsonErrorOnExceptionAttribute> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            logger.LogError($"An unhandled exception was thrown by the application.{Environment.NewLine}{exception.ToString()}");

            context.HttpContext.Response.StatusCode = 500;

            context.Result = new JsonResult(
                new
                {
                    errors = new[]
                        {
                            new
                            {
                                message = exception.Message,
                                details = exception.ToString()
                            }
                        }
                });
        }
    }
}