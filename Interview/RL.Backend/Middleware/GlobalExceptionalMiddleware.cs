namespace RL.Backend.Middleware;
/// <summary>
/// Global Exceptional Middleware is a custom middleware component designed to handle exceptions that occur during the processing of HTTP requests in an ASP.NET Core application. It captures various types of exceptions, such as OperationCanceledException, BadRequestException, NotFoundException, and any unhandled exceptions, and logs them appropriately using the ILogger interface. The middleware also generates a standardized JSON response containing problem details, including the status code, error message, stack trace (in development environment), and other relevant information. This ensures that clients receive consistent error responses while allowing developers to effectively monitor and troubleshoot issues in the application.
/// </summary>
public class GlobalExceptionalMiddleware
{
    /// <summary>
    /// RequestDelegate represents the next middleware in the HTTP request processing pipeline. It is used to invoke the next middleware component after the current middleware has completed its processing. In the context of the GlobalExceptionalMiddleware, it allows the middleware to pass control to the next component in the pipeline while also providing a mechanism to catch and handle any exceptions that may occur during the processing of the request.
    /// </summary>
    private readonly RequestDelegate _next;
    /// <summary>
    /// logger is an instance of ILogger<GlobalExceptionalMiddleware> used for logging information, warnings, and errors that occur within the middleware. It allows the middleware to record important events and exceptions, providing valuable insights for debugging and monitoring the application's behavior. The logger is injected into the middleware through dependency injection, ensuring that it can be easily configured and used throughout the application.
    /// </summary>
    private readonly ILogger<GlobalExceptionalMiddleware> _logger;
    /// <summary>
    /// environment represents the hosting environment in which the application is running. It is used to determine whether the application is in development or production mode, allowing the middleware to include additional details (such as stack traces) in error responses when in development mode. This helps developers diagnose issues more effectively while ensuring that sensitive information is not exposed in production environments. The environment is injected into the middleware through dependency injection, making it easily accessible for use within the middleware's logic.
    /// </summary>
    private readonly IHostEnvironment _env;
    /// <summary>
    /// GlobalExceptionalMiddleware represents the constructor for the GlobalExceptionalMiddleware class, which initializes the RequestDelegate, ILogger, and IHostEnvironment instances. The constructor ensures that all dependencies are provided and throws an ArgumentNullException if any of the required parameters are null. This setup allows the middleware to effectively handle exceptions that occur during HTTP request processing while maintaining a clean separation of concerns and adhering to best practices for dependency injection.
    /// </summary>
    /// <param name="next">next</param>
    /// <param name="logger">logger</param>
    /// <param name="env">environment</param>
    /// <exception cref="ArgumentNullException"></exception>
    public GlobalExceptionalMiddleware(RequestDelegate next, ILogger<GlobalExceptionalMiddleware> logger, IHostEnvironment env)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }
    /// <summary>
    /// Handles the incoming HTTP request and processes it through the middleware pipeline. It uses a try-catch block to catch specific exceptions such as OperationCanceledException, BadRequestException, NotFoundException, and any unhandled exceptions. Depending on the type of exception caught, it logs the appropriate message and generates a standardized JSON response with the relevant status code and error details. This method ensures that clients receive consistent error responses while allowing developers to effectively monitor and troubleshoot issues in the application.
    /// </summary>
    /// <param name="context">context</param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogInformation("Request was aborted by client.");
            return; // No need to write a response, client has disconnected
        }
        catch (OperationCanceledException exception)
        {
            _logger.LogWarning(exception,"Request was cancelled by client.");
            await WriteErrorAsync(context, exception, StatusCodes.Status408RequestTimeout);
        }
        catch (BadRequestException exception)
        {
            _logger.LogWarning(exception, "Bad request.");
            await WriteErrorAsync(context, exception, StatusCodes.Status400BadRequest);
        }
        catch (NotFoundException exception)
        {
            _logger.LogWarning(exception, "Not found.");
            await WriteErrorAsync(context, exception, StatusCodes.Status404NotFound);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");
            await WriteErrorAsync(context, exception, StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Writes an error response to the HTTP context based on the provided exception and status code. It checks if the response has already started, and if so, it logs a warning and does not attempt to write the error response. If the response has not started, it clears the existing response, sets the content type to "application/json", and assigns the appropriate status code. It then creates a ProblemDetails object containing details about the error, including the status code, error message, stack trace (in development environment), and other relevant information. Finally, it serializes the ProblemDetails object to JSON and writes it to the response body, ensuring that clients receive a consistent error response format.
    /// </summary>
    /// <param name="context">context</param>
    /// <param name="ex">exception</param>
    /// <param name="statusCode">status code</param>
    /// <returns></returns>
    private Task WriteErrorAsync(HttpContext context, Exception ex, int statusCode)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response has already started, cannot write error response.");
            return Task.CompletedTask;
        }
        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        ProblemDetails problemDetails = new()
        {
            Status = statusCode,
            Title = ex.Message,
            Detail = _env.IsDevelopment() ? ex.StackTrace : null,
            Type = ex.GetType().Name,
            Instance = context.Request.Path,            
            Extensions = { ["succeeded"] = false }
        };
        var opts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        var json = JsonSerializer.Serialize(problemDetails, opts);
        return context.Response.WriteAsync(json);
    }
}
/// <summary>
/// GlobalExceptionalHandlerExtensions is a static class that provides an extension method for the IApplicationBuilder interface, allowing developers to easily add the GlobalExceptionalMiddleware to the ASP.NET Core middleware pipeline. 
/// The UseGlobalExceptionalHandler method is an extension method that takes an IApplicationBuilder instance as a parameter and returns the modified IApplicationBuilder instance with the GlobalExceptionalMiddleware added to the pipeline. 
/// This design allows for a clean and convenient way to integrate global exception handling into the application, ensuring that all exceptions are consistently handled and logged throughout the application.
/// </summary>
public static class GlobalExceptionalHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionalHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionalMiddleware>();
    }
}
