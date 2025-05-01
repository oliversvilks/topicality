namespace Topicality.Client.Api.Extensions;

/// <summary>
/// Middleware for global exception management
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    /// <summary>
    /// Exception handler extension
    /// </summary>
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (contextFeature == null)
                    return;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = contextFeature.Error?.Message
                }));
            });
        });
    }
}
