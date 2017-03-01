using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using RestApiHelpers.Validation;
using Xunit;

namespace RestApiHelpers.Test.Validation
{
    internal class ValidValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return true;
        }
    }

    internal class InvalidValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return "TestErrorMessage";
        }
    }

    public class ValidateActionParametersAttributeTest
    {
        private readonly ModelStateDictionary modelStateDictionary = new ModelStateDictionary();
        private readonly RouteData routeData = new RouteData();

        private ActionExecutingContext CreateActionExecutingContext(ParameterInfo[] actionParameters)
        {
            var httpContext = new Mock<HttpContext>();
            var methodInfo = new Mock<MethodInfo>();
            var actionDescriptor = new ControllerActionDescriptor
            {
                MethodInfo = methodInfo.Object
            };

            var actionContext = new ActionContext(
                httpContext.Object,
                routeData,
                actionDescriptor,
                modelStateDictionary);

            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new object());
            
            methodInfo
                .Setup(m => m.GetParameters())
                .Returns(actionParameters);

            return actionExecutingContext;
        }

        [Fact]
        public void OnActionExecuting_NoParameters_NoModelError()
        {
            var sut = new ValidateActionParametersAttribute();
            var actionExecutingContext = CreateActionExecutingContext(new ParameterInfo[0]);

            // Act
            sut.OnActionExecuting(actionExecutingContext);

            Assert.Equal(0, actionExecutingContext.ModelState.ErrorCount);
        }

        [Fact]
        public void OnActionExecuting_ParameterWithoutValidationAttributes_NoModelError()
        {
            var sut = new ValidateActionParametersAttribute();

            var parameterInfo = new Mock<ParameterInfo>();
            parameterInfo.SetupGet(p => p.Name).Returns("test");

            var actionExecutingContext = CreateActionExecutingContext(
                new ParameterInfo[]
                {
                    parameterInfo.Object
                }
            );

            parameterInfo
                .SetupGet(p => p.CustomAttributes)
                .Returns(new List<CustomAttributeData>());

            // Act
            sut.OnActionExecuting(actionExecutingContext);

            Assert.Equal(0, actionExecutingContext.ModelState.ErrorCount);
        }

        public void MethodWithValidAttribute([ValidValidationAttribute]object arg)
        {
        }

        [Fact]
        public void OnActionExecuting_ParameterValidValidationAttributes_ModelErrorAdded()
        {
            var sut = new ValidateActionParametersAttribute();

            var parametersWithValidAttribute = this.GetType().GetMethod("MethodWithValidAttribute").GetParameters();

            var actionExecutingContext = CreateActionExecutingContext(parametersWithValidAttribute);

            // Act
            sut.OnActionExecuting(actionExecutingContext);

            Assert.Equal(0, actionExecutingContext.ModelState.ErrorCount);
        }

        public void MethodWithInvalidAttribute([InvalidValidationAttribute]object arg)
        {
        }

        [Fact]
        public void OnActionExecuting_ParameterInvalidValidationAttributes_NoModelError()
        {
            var sut = new ValidateActionParametersAttribute();

            var parametersWithInvalidAttribute = this.GetType().GetMethod("MethodWithInvalidAttribute").GetParameters();

            var actionExecutingContext = CreateActionExecutingContext(parametersWithInvalidAttribute);

            // Act
            sut.OnActionExecuting(actionExecutingContext);

            Assert.Equal(1, actionExecutingContext.ModelState.ErrorCount);
            Assert.Contains("TestErrorMessage", actionExecutingContext.ModelState.First().Value.Errors.First().ErrorMessage);
        }
    }
}