namespace RL.Backend.Commands.Handlers.Plans;
/// <summary>
/// Handles requests to remove a user or all users from a specified plan and procedure, updating user associations
/// accordingly and returning the result of the operation.
/// </summary>
/// <remarks>This handler uses the application data context to access and modify user associations with plans and
/// procedures. It logs the outcome of each removal operation for monitoring and troubleshooting purposes. The handler
/// supports both targeted removal of a specific user and bulk removal of all users associated with a plan-procedure
/// combination.</remarks>
public class RemoveUserFromPlanProcedureCommandHandler : IRequestHandler<RemoveUserFromPlanProcedureCommand, ApiResponse<Unit>>
{
    /// <summary>
    /// Provides access to the application data context within the handler.
    /// </summary>
    private readonly RLContext _context;
    /// <summary>
    /// Represents the logger instance used to record log messages for the RemoveUserFromPlanProcedureCommandHandler.
    /// </summary>
    /// <remarks>This logger is utilized to capture information, warnings, errors, and other log events
    /// related to the execution of command handling. Logging assists in monitoring application behavior and
    /// troubleshooting issues during runtime.</remarks>
    private readonly ILogger<RemoveUserFromPlanProcedureCommandHandler> _logger;
    /// <summary>
    /// Initializes a new instance of the RemoveUserFromPlanProcedureCommandHandler class using the specified database
    /// context and logger.
    /// </summary>
    /// <param name="context">The database context used to access and manipulate plan and user data. This parameter must not be null.</param>
    /// <param name="logger">The logger used to record operational and error information for this command handler. This parameter must not be
    /// null.</param>
    /// <exception cref="ArgumentNullException">Thrown when either the context or logger parameter is null.</exception>
    public RemoveUserFromPlanProcedureCommandHandler(RLContext context, ILogger<RemoveUserFromPlanProcedureCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    /// <summary>
    /// Handles the removal of a user from a specified plan-procedure association, or removes all users if no specific
    /// user is provided.
    /// </summary>
    /// <remarks>If no users are found for the specified plan and procedure, the method logs this information
    /// and returns a successful response. The method also logs the outcome of the user removal operation.</remarks>
    /// <param name="request">The command containing the identifiers for the plan, procedure, and user to be removed. If the UserId is null or
    /// zero, all users associated with the specified plan and procedure will be removed.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete, allowing the operation to be cancelled
    /// if needed.</param>
    /// <returns>An ApiResponse<Unit> indicating the success or failure of the user removal operation.</returns>
    public async Task<ApiResponse<Unit>> Handle(RemoveUserFromPlanProcedureCommand request, CancellationToken cancellationToken)
    {
        DateTime currentDateTime = DateTime.UtcNow;
        // gets all users associated with the plan and procedure
        var association = await _context.PlanProcedureUsers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(ppu => ppu.PlanId == request.PlanId && ppu.ProcedureId == request.ProcedureId && ppu.UserId == request.UserId && !ppu.IsDeleted, cancellationToken);
        // If no users are found for the specified plan and procedure, log the information and return a successful response
        if (association is null)
        {
            _logger.LogInformation("UserId {UserId} already removed or not associated with PlanId {PlanId}, ProcedureId {ProcedureId}", request.UserId, request.PlanId, request.ProcedureId);
            return ApiResponse<Unit>.Succeed(Unit.Value);
        }
        // Remove a specific user for the plan-procedure
        association.IsDeleted = true;
        association.DeletedAt = currentDateTime;
        association.UpdateDate = currentDateTime;
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("User removal successful for PlanId {PlanId}, ProcedureId {ProcedureId} and UserId {UserId}", request.PlanId, request.ProcedureId, request.UserId);
        return ApiResponse<Unit>.Succeed(Unit.Value);
    }
}