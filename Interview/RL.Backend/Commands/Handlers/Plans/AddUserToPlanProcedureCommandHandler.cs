namespace RL.Backend.Commands.Handlers.Plans;
/// <summary>
/// Handles the command to add a user to a specific plan-procedure association.
/// </summary>
/// <remarks>This handler validates the request parameters, ensuring that the PlanId, ProcedureId, and UserId are
/// positive integers and that the specified plan-procedure association exists. If the user is already associated with
/// the plan-procedure, no action is taken. Any unhandled exceptions are expected to be handled by the application's global 
/// exception handling middleware.</remarks>
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
    /// Handles the command to add a user to a specified plan procedure.
    /// </summary>
    /// <remarks>The request is validated before the user is added. If validation fails, the operation returns
    /// an appropriate response without making changes.</remarks>
    /// <param name="request">The command containing the details of the user, plan, and procedure to be added. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An ApiResponse containing the result of the operation. The response indicates success if the user is added to
    /// the plan procedure, or failure if validation does not pass.</returns>
    public async Task<ApiResponse<Unit>> Handle(AddUserToPlanProcedureCommand request, CancellationToken cancellationToken)
    {
        //validate request and check for existing association
        var validationResult = await ValidateRequestAsync(request, cancellationToken);
        if (validationResult != null)
        {
            return validationResult;
        }
        // Add the user to the plan-procedure association
        _context.PlanProcedureUsers.Add(new PlanProcedureUser
        {
            PlanId = request.PlanId,
            ProcedureId = request.ProcedureId,
            UserId = request.UserId,
            CreateDate = DateTime.UtcNow
        });
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<Unit>.Succeed(Unit.Value);
    }

    /// <summary>
    /// Validates the request to add a user to a plan-procedure association, ensuring that all identifiers are positive
    /// and that the specified entities exist.
    /// </summary>
    /// <remarks>This method checks that PlanId, ProcedureId, and UserId are positive integers and verifies
    /// the existence of the plan-procedure association and the user. If the user is already associated with the
    /// plan-procedure, a success response is returned. Validation failures are logged and returned as failure
    /// responses.</remarks>
    /// <param name="request">The command containing the PlanId, ProcedureId, and UserId to be validated for association.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous validation operation.</param>
    /// <returns>An ApiResponse<Unit> indicating the result of the validation. Returns a failure response if any validation
    /// checks fail, or null if the user is not already associated with the plan-procedure.</returns>
    private async Task<ApiResponse<Unit>?> ValidateRequestAsync(AddUserToPlanProcedureCommand request, CancellationToken cancellationToken)
    {
        if (request.PlanId <= 0)
        {
            _logger.LogWarning("Invalid input: PlanId {planId} must be a positive integers.", request.PlanId);
            return ApiResponse<Unit>.Fail(new BadRequestException("PlanId, ProcedureId and UserId must be positive integers."));
        }
        if (request.ProcedureId <= 0)
        {
            _logger.LogWarning("Invalid input: ProcedureId {ProcedureId} must be a positive integers.", request.ProcedureId);
            return ApiResponse<Unit>.Fail(new BadRequestException("ProcedureId must be positive integers."));
        }
        if (request.UserId <= 0)
        {
            _logger.LogWarning("Invalid input: UserId {UserId} must be a positive integers.", request.UserId);
            return ApiResponse<Unit>.Fail(new BadRequestException("UserId must be positive integers."));
        }
        // Check whether the plan-procedure association exists
        bool planProcedureExists = await _context.PlanProcedures.AnyAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId, cancellationToken);
        if (!planProcedureExists)
        {
            _logger.LogWarning("Invalid Plan-Procedure association: PlanId {planId}, ProcedureId {procedureId}.", request.PlanId, request.ProcedureId);
            return ApiResponse<Unit>.Fail(new BadRequestException("The specified plan-procedure association does not exist."));
        }
        // Check whether the user exists
        bool userExists = await _context.Users.AnyAsync(u => u.UserId == request.UserId, cancellationToken);
        if (!userExists)
        {
            _logger.LogWarning("Invalid User Id : {userId}.", request.UserId);
            return ApiResponse<Unit>.Fail(new BadRequestException("The specified user does not exist."));
        }
        // Check whether the user is already associated with the plan-procedure
        bool associationExists = await _context.PlanProcedureUsers.AnyAsync(ppu => ppu.PlanId == request.PlanId && ppu.ProcedureId == request.ProcedureId && ppu.UserId == request.UserId, cancellationToken);
        if (associationExists)
        {
            _logger.LogInformation("User {userId} is already associated with PlanId {planId} and ProcedureId {procedureId}. No action taken.", request.UserId, request.PlanId, request.ProcedureId);
            return ApiResponse<Unit>.Succeed(Unit.Value);
        }
        return null;
    }
}
