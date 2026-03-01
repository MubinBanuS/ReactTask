namespace RL.Backend.Controllers;
/// <summary>
/// ProceduresController is responsible for handling HTTP requests related to procedures, including retrieving all procedures from the database. It utilizes the RLContext for data access and the ILogger for logging information about the operations performed. The controller ensures that if no procedures are found, an appropriate log entry is created, and an empty list is returned to the client. This design allows for efficient handling of procedure-related requests while maintaining a clean separation of concerns and adhering to best practices for dependency injection and logging.
/// </summary>
public class ProceduresController : BaseApiController
{
    /// <summary>
    /// Represents the logger instance used to record log messages for the ProceduresController.
    /// </summary>
    private readonly ILogger<ProceduresController> _logger;
    /// <summary>
    /// Provides access to the application data context.
    /// </summary>
    private readonly RLContext _context;
    /// <summary>
    /// ProceduresController represents the constructor for the ProceduresController class, which initializes the logger and data context instances. The constructor ensures that all dependencies are provided and throws an ArgumentNullException if any of the required parameters are null. This setup allows the controller to effectively handle HTTP requests related to procedures while maintaining a clean separation of concerns and adhering to best practices for dependency injection.
    /// </summary>
    /// <param name="logger">logger</param>
    /// <param name="context">context</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ProceduresController(ILogger<ProceduresController> logger, RLContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets procedures from the database. If no procedures are found, an empty list is returned and a log entry is created.
    /// </summary>
    /// <returns>Returns all procedures</returns>
    [HttpGet]
    [EnableQuery]
    [ProducesResponseType(typeof(IEnumerable<Procedure>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IEnumerable<Procedure> Get()
    {
        IEnumerable<Procedure> procedures = _context.Procedures;
        if(procedures is null)
        {
            _logger.LogInformation("No procedures found in the database.");
            return Enumerable.Empty<Procedure>();
        }
        return procedures;
    }
}
