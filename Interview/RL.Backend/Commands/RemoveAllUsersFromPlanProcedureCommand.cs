namespace RL.Backend.Commands;
/// <summary>
/// Removes all users from a specified plan procedure association. This command is used to disassociate all users from a particular procedure within a plan, effectively clearing any user assignments for that procedure. The command requires the identifiers of both the plan and the procedure to ensure that the correct association is targeted for user removal. Upon execution, all users linked to the specified plan-procedure combination will be removed, allowing for a fresh assignment of users if needed in the future.
/// </summary>
public class RemoveAllUsersFromPlanProcedureCommand : IRequest<ApiResponse<Unit>>
{
    /// <summary>
    /// Gets or sets the identifier of the plan from which the user will be removed. This property is required and should correspond to an existing plan in the system. The value must be a positive integer.
    /// </summary>
    public int PlanId { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the procedure from which the user will be removed. This property is required and should correspond to an existing procedure in the system. The value must be a positive integer.
    /// </summary>
    public int ProcedureId { get; set; }
}
