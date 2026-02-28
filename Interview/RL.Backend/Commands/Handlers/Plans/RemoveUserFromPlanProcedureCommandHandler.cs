namespace RL.Backend.Commands.Handlers.Plans;
/// <summary>
/// Handles the removal of a user from a specified plan and procedure, or all users if the UserId is not provided.
/// Implements the IRequestHandler interface to process RemoveUserFromPlanProcedureCommand requests and return an
/// ApiResponse indicating the outcome.
/// </summary>
/// <remarks>This command handler validates the request to ensure that the PlanId and ProcedureId are positive
/// integers. If the UserId is null or zero, all users associated with the specified plan and procedure will be removed.
/// The handler logs warnings for validation failures and returns appropriate ApiResponse instances based on the outcome
/// of the operation.</remarks>
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
    /// Initializes a new instance of the RemoveUserFromPlanProcedureCommandHandler class with the specified database
    /// context and logger.
    /// </summary>
    /// <param name="context">The database context used to access and modify plan and user data.</param>
    /// <param name="logger">The logger used to record operational and error information related to user removal from a plan.</param>
    /// <exception cref="ArgumentNullException">Thrown if either the context or logger parameter is null.</exception>
    public RemoveUserFromPlanProcedureCommandHandler(RLContext context, ILogger<RemoveUserFromPlanProcedureCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task<ApiResponse<Unit>> Handle(RemoveUserFromPlanProcedureCommand request, CancellationToken cancellationToken)
    {
        //validate request
        var validationResult = ValidateRequest(request);
        if (validationResult != null)
        {
            _logger.LogWarning("RemoveUserFromPlanProcedureCommand validation failed for PlanId: {PlanId}, ProcedureId: {ProcedureId} and UserId: {UserId}: {Message}", request.PlanId, request.ProcedureId, request.UserId, validationResult.Exception?.Message);
            return validationResult;
        }
        if (request.UserId is null || request.UserId == 0)
        {
            // Remove all users for the plan-procedure; succeed if nothing to remove
            var associations = await _context.PlanProcedureUsers
                .Where(p => p.PlanId == request.PlanId && p.ProcedureId == request.ProcedureId)
                .ToListAsync(cancellationToken);
            if (associations.Count == 0)
                return ApiResponse<Unit>.Succeed(Unit.Value);
            _context.PlanProcedureUsers.RemoveRange(associations);
        }
        else
        {
            // Remove a specific user for the plan-procedure
            var association = await _context.PlanProcedureUsers
                .FindAsync(new object[] { request.PlanId, request.ProcedureId, request.UserId.Value }, cancellationToken);
            if (association is null)
                return ApiResponse<Unit>.Fail(new NotFoundException("Plan procedure association not found."));
            _context.PlanProcedureUsers.Remove(association);
        }
        var rows = await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<Unit>.Succeed(Unit.Value);
    }

    /// <summary>
    /// Validates the incoming RemoveUserFromPlanProcedureCommand request to ensure that the PlanId and ProcedureId are positive integers. 
    /// UserId can be 0 to indicate removal of all users, but cannot be negative. 
    /// If validation fails, an appropriate ApiResponse with a BadRequestException is returned; otherwise, null is returned to indicate successful validation.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private static ApiResponse<Unit>? ValidateRequest(RemoveUserFromPlanProcedureCommand request)
    {
        if (request.PlanId <= 0 || request.ProcedureId <= 0)
            return ApiResponse<Unit>.Fail(new BadRequestException("PlanId and ProcedureId must be positive integers."));

        if (request.UserId < 0)
            return ApiResponse<Unit>.Fail(new BadRequestException("UserId cannot be negative. Use 0 to remove all users."));
        return null;
    }
}