namespace RL.Backend.Commands.Validators.Plans;
/// <summary>
/// Provides validation logic for the AddProcedureToPlanCommand, ensuring that command parameters meet required criteria
/// before processing.
/// </summary>
/// <remarks>This validator enforces that both PlanId and ProcedureId are positive integers and verifies the
/// existence of the specified plan and procedure within the database context. An ArgumentNullException is thrown if the
/// provided RLContext is null. Use this class to guarantee that only valid commands are processed when adding a
/// procedure to a plan.</remarks>
public class AddProcedureToPlanCommandValidator : AbstractValidator<AddProcedureToPlanCommand>
{
    private readonly RLContext _context;
    /// <summary>
    /// Adds validation rules for the AddProcedureToPlanCommand, ensuring that PlanId and ProcedureId are positive integers. It also includes asynchronous validation to check the existence of the specified plan and procedure in the database context. An ArgumentNullException is thrown if the provided RLContext is null when initializing the validator. Use this class to validate commands before adding a procedure to a plan to prevent invalid operations.
    /// </summary>
    /// <param name="context">context used to access the database for validation checks. This parameter must not be null.</param>
    /// <exception cref="ArgumentNullException"> throws when the provided RLContext is null.</exception>
    public AddProcedureToPlanCommandValidator(RLContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleFor(x => x.PlanId).GreaterThan(0).WithMessage("PlanId must be a positive integer.");
        RuleFor(x => x.ProcedureId).GreaterThan(0).WithMessage("ProcedureId must be a positive integer.");
        RuleFor(x => x.PlanId).MustAsync(PlanExists).WithMessage("The specified plan does not exist.");
        RuleFor(x => x.ProcedureId).MustAsync(ProcedureExists).WithMessage("The specified procedure does not exist.");
    }
    /// <summary>
    /// Checks if a plan with the specified PlanId exists in the database context. This method is used as part of the validation process to ensure that the command references a valid plan before attempting to add a procedure to it.
    /// </summary>
    /// <param name="planId">Plan Id</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns true if plan exists</returns>
    private async Task<bool> PlanExists(int planId, CancellationToken cancellationToken) => await _context.Plans.AsNoTracking().AnyAsync(p => p.PlanId == planId, cancellationToken);
    /// <summary>
    /// Checks if a procedure with the specified ProcedureId exists in the database context. This method is used as part of the validation process to ensure that the command references a valid procedure before attempting to add it to a plan.
    /// </summary>
    /// <param name="procedureId">Procedure Id</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns true if procedure exists</returns>
    private async Task<bool> ProcedureExists(int procedureId, CancellationToken cancellationToken) => await _context.Procedures.AsNoTracking().AnyAsync(p => p.ProcedureId == procedureId, cancellationToken);

}
