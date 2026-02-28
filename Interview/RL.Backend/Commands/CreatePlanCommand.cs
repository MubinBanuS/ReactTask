namespace RL.Backend.Commands;
/// <summary>
/// Command to create a new plan with default values. This command does not require any parameters and will result in the creation of a new plan entity in the system when handled. The response will contain the details of the created plan, including its unique identifier and any default properties that have been set upon creation.
/// </summary>
public class CreatePlanCommand : IRequest<ApiResponse<Plan>>
{

}