namespace RL.Backend.Controllers;
/// <summary>
/// Plan Controller is responsible for handling HTTP requests related to plans, including retrieving all plans, creating new plans, and adding procedures to existing plans. It utilizes the RLContext for data access and the MediatR library for command handling, ensuring a clean separation of concerns and adherence to the CQRS pattern. The controller also supports OData queries for retrieving plans, allowing clients to filter, sort, and paginate results as needed.
/// </summary>
public class PlanController : BaseApiController
{
    /// <summary>
    /// Represents the logger instance used to record log messages for the PlanController.
    /// </summary>
    private readonly ILogger<PlanController> _logger;
    /// <summary>
    /// Provides access to the application data context.
    /// </summary>
    private readonly RLContext _context;
    /// <summary>
    /// Represents the mediator instance used to send commands and queries within the PlanController. The mediator facilitates communication between the controller and the command handlers, allowing for a clean separation of concerns and adherence to the CQRS pattern. It is used to send commands such as CreatePlanCommand and AddProcedureToPlanCommand, which are handled by their respective handlers to perform the necessary operations on the data context.
    /// </summary>
    private readonly IMediator _mediator;

    /// <summary>
    /// PlanController represents the constructor for the PlanController class, which initializes the logger, data context, and mediator instances. The constructor ensures that all dependencies are provided and throws an ArgumentNullException if any of the required parameters are null. This setup allows the controller to effectively handle HTTP requests related to plans while maintaining a clean separation of concerns and adhering to best practices for dependency injection.
    /// </summary>
    /// <param name="logger">logger</param>
    /// <param name="context">database context</param>
    /// <param name="mediator">mediator</param>
    /// <exception cref="ArgumentNullException"></exception>
    public PlanController(ILogger<PlanController> logger, RLContext context, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Gets plan data by returning an IEnumerable of Plan entities from the RLContext. 
    /// The method is decorated with the [EnableQuery] attribute, allowing clients to perform OData queries on the returned data, such as filtering, sorting, and pagination. 
    /// This provides flexibility for clients to retrieve only the data they need while optimizing performance.
    /// </summary>
    /// <returns>Returns all plans as enumerable list</returns>
    [HttpGet]
    [EnableQuery]
    [ProducesResponseType(typeof(IEnumerable<Plan>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IEnumerable<Plan> Get()
    {
        IEnumerable<Plan> plans = _context.Plans;
        if (plans is null)
        {
            _logger.LogInformation("No plans found in the database.");
            return Enumerable.Empty<Plan>();
        }
        return plans;
    }

    /// <summary>
    /// Creates plan by accepting a CreatePlanCommand which contains the necessary information to create a new plan. The command is sent to the MediatR pipeline for handling, and the response is logged accordingly. 
    /// If the operation is successful, an informational log is recorded with the newly created PlanId; if it fails, an error log with the exception details is recorded. 
    /// Finally, the response is converted to an IActionResult and returned to the client.
    /// </summary>
    /// <param name="command">Create Plan Command</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Returns response as IActionResult</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostPlan(CreatePlanCommand command, CancellationToken token)
    {
        var response = await _mediator.Send(command, token);
        if (response.Succeeded)
        {
            _logger.LogInformation("Plan created successfully with ID: {PlanId}", response.Value?.PlanId);
        }
        else
        {
            _logger.LogError(response.Exception, "Failed to create plan");
        }
        return response.ToActionResult();
    }

    /// <summary>
    /// Adds procedure to plan by accepting an AddProcedureToPlanCommand which contains the PlanId and ProcedureId. The command is sent to the MediatR pipeline for handling, and the response is logged accordingly. 
    /// If the operation is successful, an informational log is recorded; if it fails, an error log with the exception details is recorded. 
    /// Finally, the response is converted to an IActionResult and returned to the client.
    /// </summary>
    /// <param name="command">Add procedure to plan command</param>
    /// <param name="token">cancellation token</param>
    /// <returns>Returns response as IActionResult</returns>
    [HttpPost("AddProcedureToPlan")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddProcedureToPlan(AddProcedureToPlanCommand command, CancellationToken token)
    {
        var response = await _mediator.Send(command, token);
        if (response.Succeeded)
        {
            _logger.LogInformation("Procedure added to plan successfully for PlanId: {PlanId} and ProcedureId: {ProcedureId}", command.PlanId, command.ProcedureId);
        }
        else
        {
            _logger.LogError(response.Exception, "Failed to add procedure to plan for PlanId: {PlanId} and ProcedureId: {ProcedureId}", command.PlanId, command.ProcedureId);
        }
        return response.ToActionResult();
    }
}
