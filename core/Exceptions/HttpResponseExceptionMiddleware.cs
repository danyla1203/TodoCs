namespace todo.Exceptions;

public class CustomErrorHandlingMiddleware
{
  private readonly RequestDelegate _next;

  private readonly ILogger<CustomErrorHandlingMiddleware> _logger;

  public CustomErrorHandlingMiddleware(RequestDelegate next, ILogger<CustomErrorHandlingMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task Invoke(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (BaseClientException ex)
    {
      _logger.LogError(ex, "Client error");
      await HandleBaseClientExceptionAsync(context, ex);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled error");
      await HandleNonClientExceptionAsync(context, ex);
    }
  }

  private static Task HandleNonClientExceptionAsync(HttpContext context, Exception exception)
  {
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = 500;
    var errorResponse = new
    {
      StatusCode = 500,
      ErrorMessage = "Something went wrong"
    };
    return context.Response.WriteAsJsonAsync(errorResponse);
  }
  private static Task HandleBaseClientExceptionAsync(HttpContext context, BaseClientException exception)
  {
    context.Response.ContentType = "application/json";
    context.Response.StatusCode = exception.StatusCode;

    var errorResponse = new
    {
      exception.ErrorMessage,
      exception.StatusCode
    };
    return context.Response.WriteAsJsonAsync(errorResponse);
  }
}