namespace RL.Backend.Commands.Handlers.Plans;

public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, ApiResponse<Plan>>
{
    /// <summary>
    /// Provides access to the application data context within the handler.
    /// </summary>
    private readonly RLContext _context;
    /// <summary>
    /// Represents the logger instance used to record log messages for the CreatePlanCommandHandler.
    /// </summary>
    /// <remarks>This logger is utilized to capture information, warnings, errors, and other log events
    /// related to the execution of command handling. Logging assists in monitoring application behavior and
    /// troubleshooting issues during runtime.</remarks>
    private readonly ILogger<CreatePlanCommandHandler> _logger;
    /// <summary>
    /// Initializes a new instance of the CreatePlanCommandHandler class using the specified database context and
    /// logger.
    /// </summary>
    /// <param name="context">The database context used to access and manage plan-related data. This parameter must not be null.</param>
    /// <param name="logger">The logger used to record operational and diagnostic information for this handler. This parameter must not be
    /// null.</param>
    /// <exception cref="ArgumentNullException">Thrown if either the context or logger parameter is null.</exception>
    public CreatePlanCommandHandler(RLContext context, ILogger<CreatePlanCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    /// <summary>
    /// Creates new plan with default values and saves it to the database, returning the created plan in the response. 
    /// </summary>
    /// <param name="request">Create plan command</param>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>Returns created plan</returns>
    public async Task<ApiResponse<Plan>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        // Create a new plan with default values
        var plan = new Plan();
        _context.Plans.Add(plan);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Created Plan {PlanId}", plan.PlanId);
        return ApiResponse<Plan>.Succeed(plan);
    }
}