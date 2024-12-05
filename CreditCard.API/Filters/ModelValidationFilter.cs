using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace CreditCard.API.Filters
{
    public class ModelValidationFilter : IActionFilter
    {
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = "Bad Request",
                    Status = (int)StatusCodes.Status400BadRequest,
                    Detail = "Invalid request parameters.",
                    Instance = context.HttpContext.Request.Path
                };
                
                foreach (var key in context.ModelState.Keys)
                {
                    var errors = context.ModelState[key].Errors.Select(e => e.ErrorMessage).ToArray();
                    if (errors.Length > 0)
                    {
                        problemDetails.Extensions["invalidParams:" + key] = errors;
                    }
                }
                context.Result = new BadRequestObjectResult(problemDetails);
                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        { }
    }
}