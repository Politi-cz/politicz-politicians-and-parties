using politicz_politicians_and_parties.Logging;
using politicz_politicians_and_parties.Models;
using System.Net;

namespace politicz_politicians_and_parties.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILoggerAdapter<ExceptionHandlingMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (FluentValidation.ValidationException validationException)
            {
                var errorDetails = HandleValidationError(validationException);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(errorDetails);
                logger.LogWarn("Validation error");
            }
            catch (Exception exception)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new ErrorDetails()
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Internal Server Error."
                });
                logger.LogError(exception, "Unexpected error");
            }
        }

        private static ErrorDetails HandleValidationError(FluentValidation.ValidationException exception)
        {
            var error = new ErrorDetails
            {

                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Validation error"

            };
            foreach (var validationFailure in exception.Errors)
            {
                error.Errors.Add(new KeyValuePair<string, string>(
                    validationFailure.PropertyName,
                    validationFailure.ErrorMessage));
            }

            return error;
        }
    }
}
