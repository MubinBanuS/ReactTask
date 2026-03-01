namespace RL.Backend.Controllers;
/// <summary>
/// Serves as an abstract base class for API controllers, providing common configuration for routing and API versioning.
/// </summary>
/// <remarks>Controllers that inherit from this class are automatically configured with API versioning and route
/// templates, ensuring consistent endpoint structure and version management across the API. This class should be used
/// as a foundation for all API controllers in the application.</remarks>
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase
{

}
