namespace PoliticiansAndParties.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ILoggerAdapter<ExceptionHandlingMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException validationException)
        {
            var errorDetails = HandleValidationError(validationException);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(errorDetails);
            logger.LogWarn("Validation error");
        }
        catch (Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new ErrorDetail("Internal Server Error."));
            logger.LogError(exception, "Unexpected error");
        }
    }

    private static ErrorDetail HandleValidationError(ValidationException exception)
    {
        var errors = exception.Errors.GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());

        return new ErrorDetail("Validation error", errors);
    }
}
