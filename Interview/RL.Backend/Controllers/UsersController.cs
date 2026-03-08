namespace RL.Backend.Controllers;
/// <summary>
/// UserController is responsible for handling HTTP requests related to users, including retrieving all users from the database. It utilizes the RLContext for data access and the ILogger for logging information about the operations performed. The controller ensures that if no users are found, an appropriate log entry is created, and an empty list is returned to the client. This design allows for efficient handling of user-related requests while maintaining a clean separation of concerns and adhering to best practices for dependency injection and logging.
/// </summary>
public class UsersController : BaseApiController
{
    /// <summary>
    /// Represents the logger instance used to record log messages for the UsersController.
    /// </summary>
    private readonly ILogger<UsersController> _logger;
    /// <summary>
    /// Provides access to the application data context.
    /// </summary>
    private readonly RLContext _context;
    /// <summary>
    /// UsersController represents the constructor for the UsersController class, which initializes the logger and data context instances. The constructor ensures that all dependencies are provided and throws an ArgumentNullException if any of the required parameters are null. This setup allows the controller to effectively handle HTTP requests related to users while maintaining a clean separation of concerns and adhering to best practices for dependency injection.
    /// </summary>
    /// <param name="logger">logger</param>
    /// <param name="context">database context</param>
    /// <exception cref="ArgumentNullException"></exception>
    public UsersController(ILogger<UsersController> logger, RLContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Get all users from the database. If no users are found, an empty list is returned and a log entry is created to indicate that no users were found. This method ensures that the client receives a consistent response regardless of the state of the database, while also providing valuable information in the logs for debugging and monitoring purposes. 
    /// </summary>
    /// <returns>Returns users list</returns>
    [HttpGet]
    [EnableQuery]
    [ProducesResponseType(typeof(IEnumerable<Plan>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IEnumerable<User> Get()
    {
        IEnumerable<User> users = _context.Users;
        if(users is null)
        {
            _logger.LogInformation("Users collection is null in the database context.");
            return Enumerable.Empty<User>();
        }
        return users;
    }
}
