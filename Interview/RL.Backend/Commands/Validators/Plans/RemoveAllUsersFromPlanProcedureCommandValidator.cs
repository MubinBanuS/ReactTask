namespace RL.Backend.Commands.Validators.Plans;
/// <summary>
/// Provides validation logic for the RemoveAllUsersFromPlanProcedureCommand to ensure that command parameters meet required
/// </summary>
/// <remarks>This validator enforces that PlanId and ProcedureId are positive integers and verifies the existence of the specified plan-procedure association. 
/// Use this class to validate commands prior to removing all users from a plan procedure to prevent invalid operations.</remarks>
public class RemoveAllUsersFromPlanProcedureCommandValidator : AbstractValidator<RemoveAllUsersFromPlanProcedureCommand>
{
    private readonly RLContext _context;
    /// <summary>
    /// Adds validation rules for the RemoveAllUsersFromPlanProcedureCommand, ensuring that PlanId and ProcedureId are positive integers. It also includes asynchronous validation to check the existence of the specified plan-procedure association in the database context. An ArgumentNullException is thrown if the provided RLContext is null when initializing the validator. Use this class to validate commands before removing all users from a plan procedure to prevent invalid operations.         
    /// </summary>
    /// <param name="context">context used to access the database for validation checks. This parameter must not be null.</param>
    /// <exception cref="ArgumentNullException"> throws when the provided RLContext is null.</exception>
    public RemoveAllUsersFromPlanProcedureCommandValidator(RLContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleFor(x => x.PlanId).GreaterThan(0).WithMessage("PlanId must be a positive integer.");
        RuleFor(x => x.ProcedureId).GreaterThan(0).WithMessage("ProcedureId must be a positive integer.");
        RuleFor(x => x).MustAsync(PlanProcedureExists).WithMessage("The specified plan-procedure association does not exist.");
    }
    /// <summary>
    /// Checks asynchronously if a plan-procedure association exists in the database context based on the provided PlanId and ProcedureId in the command. This method is used as part of the validation rules to ensure that the specified association exists before attempting to remove all users from it. It returns true if the association exists, and false otherwise. Use this method to validate the existence of the plan-procedure association before performing any operations.
    /// </summary>
    /// <param name="request">Remove All Users from Plan Procedure Command</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns true if plan procedure association exists</returns>
    private async Task<bool> PlanProcedureExists(RemoveAllUsersFromPlanProcedureCommand request, CancellationToken cancellationToken) => await _context.PlanProcedures.AsNoTracking().AnyAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId, cancellationToken);
}
