using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using politicz_politicians_and_parties.Models;
using System;
using System.Net;

namespace politicz_politicians_and_parties.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        // https://code-maze.com/global-error-handling-aspnetcore/
        public static void ConfigureExceptionHandler(this IApplicationBuilder app/*, ILoggerManager logger*/)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        /*logger.LogError($"Something went wrong: {contextFeature.Error}");*/

                        if (contextFeature.Error is ValidationException)
                        {
                            // TODO Tidy up and add switch for another exceptions if needed
                            var exception = (ValidationException)contextFeature.Error;
                            context.Response.StatusCode = 400;

                            var error = new ValidationProblemDetails
                            {
                                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                                Status = 400,
                                Extensions =
                                {
                                    ["traceId"] = context.TraceIdentifier
                                }
                            };
                            foreach (var validationFailure in exception.Errors)
                            {
                                error.Errors.Add(new KeyValuePair<string, string[]>(
                                    validationFailure.PropertyName,
                                    new[] { validationFailure.ErrorMessage }));
                            }
                            await context.Response.WriteAsJsonAsync(error);
                        }
                        else { 
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = "Internal Server Error."
                            }.ToString());
                        }

                    }
                });
            });
        }
    }
}
