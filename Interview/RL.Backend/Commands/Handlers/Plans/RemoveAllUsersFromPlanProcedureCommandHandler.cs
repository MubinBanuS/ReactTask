namespace RL.Backend.Commands.Handlers.Plans;
/// <summary>
/// Handles requests to remove all users from a specified plan and procedure, updating user associations accordingly and returning the result of the operation.
/// </summary>
public class RemoveAllUsersFromPlanProcedureCommandHandler : IRequestHandler<RemoveAllUsersFromPlanProcedureCommand, ApiResponse<Unit>>
{
    /// <summary>
    /// Provides access to the application data context within the handler.
    /// </summary>
    private readonly RLContext _context;
    /// <summary>
    /// Represents the logger instance used to record log messages for the RemoveAllUsersFromPlanProcedureCommandHandler.
    /// </summary>
    /// <remarks>This logger is utilized to capture information, warnings, errors, and other log events
    /// related to the execution of command handling. Logging assists in monitoring application behavior and
    /// troubleshooting issues during runtime.</remarks>
    private readonly ILogger<RemoveAllUsersFromPlanProcedureCommandHandler> _logger;
    /// <summary>
    /// Initializes a new instance of the RemoveAllUsersFromPlanProcedureCommandHandler class using the specified database
    /// </summary>
    /// <param name="context">The database context used to access and manipulate plan and user data. This parameter must not be null.</param>
    /// <param name="logger">The logger used to record operational and error information for this command handler. This parameter must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when either the context or logger parameter is null.</exception>
    public RemoveAllUsersFromPlanProcedureCommandHandler(RLContext context, ILogger<RemoveAllUsersFromPlanProcedureCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    /// <summary>
    /// Handles the RemoveAllUsersFromPlanProcedureCommand by removing all user associations for the specified plan and procedure.
    /// </summary>
    /// <param name="request">The command containing the identifiers for the plan and procedure. If the plan-procedure associations are found, they will be marked as deleted. If no associations are found, the operation will succeed without making changes. If the number of users to remove is not
    /// zero, all users associated with the specified plan and procedure will be removed.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete, allowing the operation to be cancelled
    /// if needed.</param>
    /// <returns>An ApiResponse<Unit> indicating the success or failure of the user removal operation.</returns>
    public async Task<ApiResponse<Unit>> Handle(RemoveAllUsersFromPlanProcedureCommand request, CancellationToken cancellationToken)
    {
        DateTime currentDateTime = DateTime.UtcNow;
        // gets all users associated with the plan and procedure
        var associations = await _context.PlanProcedureUsers
            .IgnoreQueryFilters()
            .Where(p => p.PlanId == request.PlanId && p.ProcedureId == request.ProcedureId && !p.IsDeleted)
            .ToListAsync(cancellationToken);
        // if no users are found, log the information and return a successful response without making changes
        if (associations.Count == 0)
        {
            _logger.LogInformation("No users found for PlanId {PlanId}, ProcedureId {ProcedureId}", request.PlanId, request.ProcedureId);
            return ApiResponse<Unit>.Succeed(Unit.Value);
        }
        // mark all found associations as deleted and update the relevant timestamps, then save the changes to the database
        foreach (var association in associations)
        {
            association.IsDeleted = true;
            association.DeletedAt = currentDateTime;
            association.UpdateDate = currentDateTime;
        }
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("User removal successful for PlanId {PlanId} and ProcedureId {ProcedureId}", request.PlanId, request.ProcedureId);
        return ApiResponse<Unit>.Succeed(Unit.Value);
    }
}