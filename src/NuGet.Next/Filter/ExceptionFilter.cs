using NuGet.Next.Core.Exceptions;
using NuGet.Next.Protocol.Models;

namespace NuGet.Next.Filter;

public class ExceptionFilter(ILogger<ExceptionFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (NotFoundException notFoundException)
        {
            context.HttpContext.Response.StatusCode = 404;
            context.HttpContext.Response.ContentType = "application/json";

            logger.LogWarning("Resource not found: {Path}", context.HttpContext.Request.Path);
            return new OkResponse(false, notFoundException.Message);
        }
        catch (UnauthorizedAccessException)
        {
            context.HttpContext.Response.StatusCode = 401;
            context.HttpContext.Response.ContentType = "application/json";

            var response = new OkResponse(false, "未授权的访问");

            logger.LogWarning("Unauthorized access to {Path}", context.HttpContext.Request.Path);

            return response;
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                case InvalidOperationException or BadRequestException:
                {
                    context.HttpContext.Response.StatusCode = 200;
                    context.HttpContext.Response.ContentType = "application/json";

                    var response = new OkResponse(false, ex.Message);

                    logger.LogWarning("Invalid operation: {Message}", ex.Message);

                    return response;
                }
                case ForbiddenException:
                {
                    context.HttpContext.Response.StatusCode = 403;
                    context.HttpContext.Response.ContentType = "application/json";

                    var response = new OkResponse(false, ex.Message);


                    logger.LogWarning("Forbidden access to {Path}", context.HttpContext.Request.Path);

                    return response;
                }
                default:
                {
                    context.HttpContext.Response.StatusCode = 500;
                    context.HttpContext.Response.ContentType = "application/json";

                    var response = new OkResponse(false, "服务器内部错误");

                    logger.LogError(ex, "An error occurred while processing {Path}", context.HttpContext.Request.Path);

                    return response;
                }
            }
        }
    }
}