namespace RL.Backend.Commands.Handlers.Plans;
/// <summary>
/// Handles requests to associate a user with a specific plan and procedure, ensuring that existing associations are
/// properly managed according to their current state.
/// </summary>
/// <remarks>This handler supports creating new associations, restoring soft-deleted associations, and avoids
/// duplicating active associations. It is typically used in scenarios where users need to be mapped to plans and
/// procedures, and ensures data integrity by checking for existing records before performing operations. Logging is
/// performed for all major actions to aid in monitoring and troubleshooting.</remarks>
public class AddUserToPlanProcedureCommandHandler : IRequestHandler<AddUserToPlanProcedureCommand, ApiResponse<Unit>>
{
    /// <summary>
    /// Provides access to the application data context within the handler.
    /// </summary>
    private readonly RLContext _context;
    /// <summary>
    /// Represents the logger instance used to record log messages for the AddUserToPlanProcedureCommandHandler.
    /// </summary>
    /// <remarks>This logger is utilized to capture information, warnings, errors, and other log events
    /// related to the execution of command handling. Logging assists in monitoring application behavior and
    /// troubleshooting issues during runtime.</remarks>
    private readonly ILogger<AddUserToPlanProcedureCommandHandler> _logger;
    /// <summary>
    /// Initializes a new instance of the AddUserToPlanProcedureCommandHandler class using the specified database
    /// context and logger.
    /// </summary>
    /// <param name="context">The database context used to access and manage plan-related data. Cannot be null.</param>
    /// <param name="logger">The logger used to record operational and error information for this command handler. Cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if either the context or logger parameter is null.</exception>
    public AddUserToPlanProcedureCommandHandler(RLContext context, ILogger<AddUserToPlanProcedureCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    /// <summary>
    /// Handles the addition of a user to a specified plan and procedure, ensuring that existing associations are
    /// managed appropriately.
    /// </summary>
    /// <remarks>If an association already exists, the method checks whether it is active or soft-deleted. If
    /// active, it returns success without making changes. If soft-deleted, it restores the association. If no
    /// association exists, it creates a new one.</remarks>
    /// <param name="request">The command containing the identifiers for the plan, procedure, and user to associate.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>An ApiResponse indicating the success of the operation. Returns a Unit value if the association is created,
    /// restored, or already active.</returns>
    public async Task<ApiResponse<Unit>> Handle(AddUserToPlanProcedureCommand request, CancellationToken cancellationToken)
    {
        DateTime currentDateTime = DateTime.UtcNow;
        //Check if association already exists (including soft-deleted associations)
        var existingAssociation = await _context.PlanProcedureUsers.IgnoreQueryFilters().FirstOrDefaultAsync(ppu => ppu.PlanId == request.PlanId && ppu.ProcedureId == request.ProcedureId && ppu.UserId == request.UserId, cancellationToken);
        //Association exists, check if it's active or soft-deleted
        if (existingAssociation is not null)
        {
            //case 1: Association exists and is active, return success without making changes
            if (!existingAssociation.IsDeleted)
            {
                _logger.LogInformation("User {userId} is already associated with PlanId {planId} and ProcedureId {procedureId}.", request.UserId, request.PlanId, request.ProcedureId);
                return ApiResponse<Unit>.Succeed(Unit.Value);
            }
            //case 2: Association exists but is soft-deleted, restore the association
            else
            {
                existingAssociation.IsDeleted = false;
                existingAssociation.DeletedAt = null;
                existingAssociation.UpdateDate = currentDateTime;
                _logger.LogInformation("Restoring soft-deleted association for User {userId} with PlanId {planId} and ProcedureId {procedureId}.", request.UserId, request.PlanId, request.ProcedureId);
            }
        }
        //case 3: Association does not exist, create a new association
        else
        {
            _context.PlanProcedureUsers.Add(new PlanProcedureUser
            {
                PlanId = request.PlanId,
                ProcedureId = request.ProcedureId,
                UserId = request.UserId,
                CreateDate = currentDateTime,
                IsDeleted = false
            });
             _logger.LogInformation("Creating new association for User {userId} with PlanId {planId} and ProcedureId {procedureId}.", request.UserId, request.PlanId, request.ProcedureId);
        }
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("UserId {UserId} mapped to PlanId {PlanId} and ProcedureId {ProcedureId} successfully", request.UserId, request.PlanId, request.ProcedureId);
        return ApiResponse<Unit>.Succeed(Unit.Value);
    }
}
