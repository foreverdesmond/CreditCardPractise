using NLog;

namespace CreditCard.API.ErrorHandling
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        public GlobalExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return context.Response.WriteAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error. Please try again later." + exception
            }.ToString()); ;
        }
    }
}