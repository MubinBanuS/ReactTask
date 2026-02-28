namespace RL.Backend.Commands;
/// <summary>
/// Represents a command to add a user to a specific plan-procedure association. This command is used to associate a user with a particular procedure within a given plan, allowing for the management of user assignments to various procedures in the context of a plan. The command requires the identifiers for the plan, procedure, and user to be specified, and it returns an ApiResponse indicating the success or failure of the operation.
/// </summary>
public class AddUserToPlanProcedureCommand : IRequest<ApiResponse<Unit>>
{
    /// <summary>
    /// Gets or sets the identifier of the plan to which the procedure will be added. This property is required and should correspond to an existing plan in the system. The value must be a positive integer.
    /// </summary>
    public int PlanId { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the procedure to be added to the plan. This property is required and should correspond to an existing procedure in the system. The value must be a positive integer.
    /// </summary>
    public int ProcedureId { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the user to be associated with the specified plan-procedure. This property is required and should correspond to an existing user in the system. The value must be a positive integer, and it indicates which user is being added to the plan-procedure association.
    /// </summary>
    public int UserId { get; set; }
}
