namespace RL.Backend.Commands;
/// <summary>
/// Command to remove a user from a specific plan-procedure association. This command is used to disassociate a user from a particular procedure within a given plan, allowing for the management of user assignments to various procedures in the context of a plan. The command requires the identifiers for the plan, procedure, and optionally the user to be specified. If the UserId is not provided or is zero, all users associated with the specified plan and procedure will be removed. The command returns an ApiResponse indicating the success or failure of the operation.
/// </summary>
public class RemoveUserFromPlanProcedureCommand : IRequest<ApiResponse<Unit>>
{
    /// <summary>
    /// Gets or sets the identifier of the plan from which the user will be removed. This property is required and should correspond to an existing plan in the system. The value must be a positive integer.
    /// </summary>
    public int PlanId { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the procedure from which the user will be removed. This property is required and should correspond to an existing procedure in the system. The value must be a positive integer.
    /// </summary>
    public int ProcedureId { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the user to be removed from the specified plan-procedure association. This property is optional; if not provided or set to zero, all users associated with the specified plan and procedure will be removed. If provided, the value must correspond to an existing user in the system and should be a positive integer, indicating which user is being removed from the plan-procedure association.
    /// </summary>
    /// <remarks> UserId is nullable to allow for the removal of all users from the specified plan-procedure association when UserId is not provided or set to zero. 
    /// If UserId is null or zero, the command handler will interpret this as an instruction to remove all users associated with the given plan and procedure. 
    /// </remarks>
    public int? UserId { get; set; }
}
