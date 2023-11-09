using System.Net;
using WebApi.Dtos;

namespace WebApi.Middlewere
{
    public class ExceptionHandlingMiddlwere : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddlwere> _logger;

        public ExceptionHandlingMiddlwere(ILogger<ExceptionHandlingMiddlwere> logger) =>
            _logger = logger;

        // Middleware function to handle exceptions
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // Log the exception stack trace along with the message
                _logger.LogError(ex, ex.Message);
                await HandleExeptionAsync(context, ex.Message, HttpStatusCode.InternalServerError);
            }
        }

        // Function to handle exceptions and send a response
        private async Task HandleExeptionAsync(HttpContext context, string exMassage, HttpStatusCode httpStatusCode)
        {
            HttpResponse response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)httpStatusCode;

            ErrorDto errorDto = new()
            {
                Message = exMassage,
                StatusCode = (int)httpStatusCode,
            };

            await response.WriteAsJsonAsync(errorDto);
        }
    }
}
