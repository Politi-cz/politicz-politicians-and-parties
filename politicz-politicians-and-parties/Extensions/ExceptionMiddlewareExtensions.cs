using Microsoft.AspNetCore.Diagnostics;
using politicz_politicians_and_parties.Models;
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
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error."
                        }.ToString());

                    }
                });
            });
        }
    }
}
