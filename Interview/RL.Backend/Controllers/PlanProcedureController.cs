namespace RL.Backend.Controllers;
/// <summary>
/// PlanProcedureController is responsible for handling HTTP requests related to plan procedures, including retrieving all plan procedures, adding users to a plan procedure, and removing users from a plan procedure. It utilizes the RLContext for data access and the MediatR library for command handling, ensuring a clean separation of concerns and adherence to the CQRS pattern. The controller also supports OData queries for retrieving plan procedures, allowing clients to filter, sort, and paginate results as needed.
/// </summary>
public class PlanProcedureController : BaseApiController
{
    /// <summary>
    /// Represents the logger instance used to record log messages for the PlanProcedureController.
    /// </summary>
    private readonly ILogger<PlanProcedureController> _logger;
    /// <summary>
    /// Provides access to the application data context.
    /// </summary>
    private readonly RLContext _context;
    /// <summary>
    /// Represents the mediator instance used to send commands and queries within the PlanProcedureController. The mediator facilitates communication between the controller and the command handlers, allowing for a clean separation of concerns and adherence to the CQRS pattern. It is used to send commands such as CreatePlanCommand and AddProcedureToPlanCommand, which are handled by their respective handlers to perform the necessary operations on the data context.
    /// </summary>
    private readonly IMediator _mediator;
    /// <summary>
    /// PlanProcedureController represents the constructor for the PlanProcedureController class, which initializes the logger, data context, and mediator instances. The constructor ensures that all dependencies are provided and throws an ArgumentNullException if any of the required parameters are null. This setup allows the controller to effectively handle HTTP requests related to plans while maintaining a clean separation of concerns and adhering to best practices for dependency injection.
    /// </summary>
    /// <param name="logger">logger</param>
    /// <param name="context">database context</param>
    /// <param name="mediator">mediator</param>
    /// <exception cref="ArgumentNullException"></exception>
    public PlanProcedureController(ILogger<PlanProcedureController> logger, RLContext context, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Gets all plan procedures. This endpoint retrieves a list of all plan procedures available in the system. 
    /// Each plan procedure represents a specific procedure that is part of a plan, and the response includes details such as the procedure ID, plan ID, and any associated users assigned to that procedure.
    /// </summary>
    /// <returns>Returns all plan procedures as enumerable list</returns>
    [HttpGet]
    [EnableQuery]
    [ProducesResponseType(typeof(IEnumerable<PlanProcedure>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IEnumerable<PlanProcedure> Get()
    {
        IEnumerable<PlanProcedure> planProcedures = _context.PlanProcedures;
        if(planProcedures is null)
        {
            _logger.LogInformation("No plan procedures found in the database.");
            return Enumerable.Empty<PlanProcedure>();
        }
        return planProcedures;
    }

    /// <summary>
    /// Adds user to a plan procedure. This endpoint is used to assign a user to a specific procedure within a plan. The request body should contain the PlanId, ProcedureId, and UserId to specify which user is being added to which procedure in which plan.
    /// </summary>
    /// <param name="command">Add user to plan procedure command</param>
    /// <param name="token">cancellation token</param>
    /// <returns>Returns response as IActionResult.</returns>
    [HttpPost("AddUser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] AddUserToPlanProcedureCommand command, CancellationToken token)
    {
        var response = await _mediator.Send(command, token);
        if (response.Succeeded)
        {
            _logger.LogInformation("User with id {UserId} added to procedure with id {ProcedureId} in plan with id {PlanId}", command.UserId, command.ProcedureId, command.PlanId);
        }
        else
        {
            _logger.LogError(response.Exception, "Failed to add user with id {UserId} to procedure with id {ProcedureId} in plan with id {PlanId}", command.UserId, command.ProcedureId, command.PlanId);
        }
        return response.ToActionResult();
    }

    /// <summary>
    /// Removes a user from plan procedure. This endpoint is used to unassign a user from a specific procedure within a plan. The request body should contain the PlanId, ProcedureId, and UserId to specify which user is being removed from which procedure in which plan.
    /// </summary>
    /// <param name="command">Remove user from plan procedure command</param>
    /// <param name="token">cancellation token</param>
    /// <returns>Returns response as IActionResult.</returns>
    [HttpDelete("RemoveUser")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromBody] RemoveUserFromPlanProcedureCommand command, CancellationToken token)
    {
        var response = await _mediator.Send(command, token);
        if (response.Succeeded)
        {
            _logger.LogInformation("User with id {UserId} removed from procedure with id {ProcedureId} in plan with id {PlanId}", command.UserId, command.ProcedureId, command.PlanId);
        }
        else
        {
            _logger.LogError(response.Exception, "Failed to remove user with id {UserId} from procedure with id {ProcedureId} in plan with id {PlanId}", command.UserId, command.ProcedureId, command.PlanId);
        }
        return response.ToActionResult();
    }
    /// <summary>
    /// Removes all users from plan procedure. This endpoint is used to unassign all users from a specific procedure within a plan. The request body should contain the PlanId and ProcedureId.
    /// </summary>
    /// <param name="command">Remove all users from plan procedure command</param>
    /// <param name="token">cancellation token</param>
    /// <returns>Returns response as IActionResult.</returns>
    [HttpDelete("RemoveAllUsers")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAll([FromBody] RemoveAllUsersFromPlanProcedureCommand command, CancellationToken token)
    {
        var response = await _mediator.Send(command, token);
        if (response.Succeeded)
        {
            _logger.LogInformation("All Users are removed from procedure with id {ProcedureId} in plan with id {PlanId}", command.ProcedureId, command.PlanId);
        }
        else
        {
            _logger.LogError(response.Exception, "Failed to remove all users from procedure with id {ProcedureId} in plan with id {PlanId}", command.ProcedureId, command.PlanId);
        }
        return response.ToActionResult();
    }

}
