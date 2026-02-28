namespace RL.Backend.Commands;
/// <summary>
/// Represents a command to add a procedure to a specified plan.
/// </summary>
public class AddProcedureToPlanCommand : IRequest<ApiResponse<Unit>>
{
    /// <summary>
    /// Gets or sets the identifier of the plan to which the procedure will be added. This property is required and should correspond to an existing plan in the system. The value must be a positive integer.
    /// </summary>
    public int PlanId { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the procedure to be added to the plan. This property is required and should correspond to an existing procedure in the system. The value must be a positive integer.
    /// </summary>
    public int ProcedureId { get; set; }
}