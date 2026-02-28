namespace RL.Backend.Commands.Handlers.Plans;
/// <summary>
/// Handles requests to add a procedure to a specified plan, ensuring input validation and preventing duplicate
/// associations.
/// </summary>
/// <remarks>This handler processes AddProcedureToPlanCommand requests by validating the provided plan and
/// procedure identifiers, checking for their existence, and associating the procedure with the plan if not already
/// linked. It returns appropriate error responses if validation fails or if either the plan or procedure does not
/// exist. The handler is intended for use within a CQRS architecture and relies on a data context and logger for
/// operation and diagnostics. Any unhandled exceptions are expected to be handled by the application's global exception handling middleware.</remarks>
public class AddProcedureToPlanCommandHandler : IRequestHandler<AddProcedureToPlanCommand, ApiResponse<Unit>>
{
    /// <summary>
    /// Provides access to the application data context within the handler.
    /// </summary>
    private readonly RLContext _context;
    /// <summary>
    /// Represents the logger instance used to record log messages for the AddProcedureToPlanCommandHandler.
    /// </summary>
    /// <remarks>This logger is utilized to capture information, warnings, errors, and other log events
    /// related to the execution of command handling. Logging assists in monitoring application behavior and
    /// troubleshooting issues during runtime.</remarks>
    private readonly ILogger<AddProcedureToPlanCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the AddProcedureToPlanCommandHandler class using the specified data context and
    /// logger.
    /// </summary>
    /// <remarks>Both parameters must be provided and properly configured to ensure correct operation of the
    /// command handler. The context should be set up to access the required data store, and the logger should be
    /// suitable for capturing relevant events.</remarks>
    /// <param name="context">The RLContext instance used to access and manage data related to plan procedures.</param>
    /// <param name="logger">The ILogger instance used to log operational and diagnostic information for this command handler.</param>
    public AddProcedureToPlanCommandHandler(RLContext context, ILogger<AddProcedureToPlanCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the addition of a procedure to a specified plan, validating the request and ensuring that both the plan
    /// and procedure exist before creating the association.
    /// </summary>
    /// <remarks>This method checks for the existence of the plan and procedure before attempting to create an
    /// association. If the procedure is already associated with the plan, it will return a success response without
    /// making any changes.</remarks>
    /// <param name="request">The command containing the details of the procedure to be added to the plan, including the identifiers for both
    /// the plan and the procedure.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to propagate notification that operations should be canceled.</param>
    /// <returns>An ApiResponse<Unit> indicating the success or failure of the operation, including validation errors or not
    /// found exceptions if applicable.</returns>
    public async Task<ApiResponse<Unit>> Handle(AddProcedureToPlanCommand request, CancellationToken cancellationToken)
    {
        //validate request and check for existing association
        var validationResult = await ValidateRequestAsync(request, cancellationToken);
        if (validationResult != null)
        {
            _logger.LogWarning("AddProcedureToPlanCommand validation failed for PlanId: {PlanId} and Procedure: {ProcedureId}: {Message}", request.PlanId, request.ProcedureId, validationResult.Exception?.Message);
            return validationResult;
        }
        //Add the procedure to the plan
        _context.PlanProcedures.Add(new PlanProcedure
        {
            PlanId = request.PlanId,
            ProcedureId = request.ProcedureId
        });
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<Unit>.Succeed(Unit.Value);
    }

    /// <summary>
    /// Validates a request to associate a procedure with a plan, ensuring that both entities exist and are not already
    /// linked.
    /// </summary>
    /// <remarks>This method checks for valid identifiers, verifies the existence of the plan and procedure,
    /// and determines whether the procedure is already linked to the plan. Logging is performed for missing entities
    /// and existing associations.</remarks>
    /// <param name="request">The command containing the identifiers for the plan and procedure to be validated. PlanId and ProcedureId must
    /// be positive integers.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the asynchronous operation.</param>
    /// <returns>An ApiResponse<Unit> indicating the result of the validation. Returns a failure response if validation fails, or
    /// a success response if the procedure is already associated with the plan. Returns null if the procedure is not
    /// yet associated and validation passes.</returns>
    private async Task<ApiResponse<Unit>?> ValidateRequestAsync(AddProcedureToPlanCommand request, CancellationToken cancellationToken)
    {
        //Validate request
        if (request.PlanId < 1)
            return ApiResponse<Unit>.Fail(new BadRequestException("PlanId must be a positive integer"));
        if (request.ProcedureId < 1)
            return ApiResponse<Unit>.Fail(new BadRequestException("ProcedureId must be a positive integer"));
        bool planExists = await _context.Plans.AnyAsync(p => p.PlanId == request.PlanId, cancellationToken);
        if (!planExists)
        {
            _logger.LogWarning("PlanId: {PlanId} not found when attempting to add ProcedureId: {ProcedureId}", request.PlanId, request.ProcedureId);
            return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} not found"));
        }

        bool procedureExists = await _context.Procedures.AnyAsync(p => p.ProcedureId == request.ProcedureId, cancellationToken);
        if (!procedureExists)
        {
            _logger.LogWarning("ProcedureId: {ProcedureId} not found when attempting to add to PlanId: {PlanId}", request.ProcedureId, request.PlanId);
            return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found"));
        }
        //Check if the procedure is already associated with the plan
        bool associationExists = await _context.PlanProcedures.AnyAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId, cancellationToken);

        //Already has the procedure, so just succeed
        if (associationExists)
        {
            _logger.LogInformation("ProcedureId: {ProcedureId} is already associated with PlanId: {PlanId}. No action taken.", request.ProcedureId, request.PlanId);
            return ApiResponse<Unit>.Succeed(Unit.Value);
        }
        return null;
    }
}