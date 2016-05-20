using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using RestApiHelpers.Validation;
using System.Collections.Generic;
using Xunit;

namespace RestApiHelpers.Test.Validation
{
    public class ReturnBadRequestOnModelErrorTest
    {
        [Fact]
        public void OnActionExecuting_ModelValid_NoResultSet()
        {
            var sut = new ReturnBadRequestOnModelError();

            var modelStateDictionary = new ModelStateDictionary();
            var httpContext = new Mock<HttpContext>();
            var actionContext = new ActionContext(httpContext.Object, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(), modelStateDictionary);
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new object());

            // Act
            sut.OnActionExecuting(actionExecutingContext);

            Assert.Null(actionExecutingContext.Result);
        }

        [Fact]
        public void OnActionExecuting_ModelInvalid_BadRequestResultSet()
        {
            var sut = new ReturnBadRequestOnModelError();

            var modelStateDictionary = new ModelStateDictionary();
            var httpContext = new Mock<HttpContext>();
            var actionContext = new ActionContext(httpContext.Object, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor(), modelStateDictionary);
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new object());
            modelStateDictionary.AddModelError("error", "error");

            // Act
            sut.OnActionExecuting(actionExecutingContext);

            Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);
        }
    }
}