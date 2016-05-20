using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestApiHelpers.Validation
{
    /// <summary>
    /// If there is a model error during executing an action, it returns a Bad Request response with the model details.
    /// </summary>
    public class ReturnBadRequestOnModelError : ActionFilterAttribute
    {
        public ReturnBadRequestOnModelError()
        {
            Order = 2;
        }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Result = new BadRequestObjectResult(actionContext.ModelState);
            }
        }
    }
}