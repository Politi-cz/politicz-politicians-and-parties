using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using politicz_politicians_and_parties.Models;
using System.Net;

namespace politicz_politicians_and_parties.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, Logging.ILoggerAdapter<object> logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        switch (contextFeature.Error)
                        {
                            case ValidationException validationException:
                                var errorDetails = HandleValidationError(validationException);
                                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                await context.Response.WriteAsJsonAsync(errorDetails);
                                logger.LogWarn("Validation error");
                                break;
                            default:
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                await context.Response.WriteAsJsonAsync(new ErrorDetails()
                                {
                                    StatusCode = context.Response.StatusCode,
                                    Message = "Internal Server Error."
                                });
                                logger.LogError(contextFeature.Error, "Unexpected error");
                                break;

                        };
                    }
                });
            });
        }

        private static ErrorDetails HandleValidationError(ValidationException exception)
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
