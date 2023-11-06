using System.Net;
using WebApi.Dtos;
namespace WebApi.Middlewere;
public class ExceptionHandlingMiddlwere : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddlwere> _logger;
    public ExceptionHandlingMiddlwere(
        ILogger<ExceptionHandlingMiddlwere> logger) =>
        _logger = logger;
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExeptionAsync(context, ex.Message,
                  HttpStatusCode.InternalServerError);
        }
    }

    private async Task HandleExeptionAsync(HttpContext context,
        string exMassage, HttpStatusCode httpStatusCode)
    {
        _logger.LogError(exMassage);

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