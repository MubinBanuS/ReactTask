namespace RL.Backend.Commands.Validators.Plans;
/// <summary>
/// Provides validation logic for the AddUserToPlanProcedureCommand to ensure that command parameters meet required
/// criteria before processing.
/// </summary>
/// <remarks>This validator enforces that PlanId, ProcedureId, and UserId are positive integers, and verifies the
/// existence of the specified plan-procedure association and user in the database. An ArgumentNullException is thrown
/// if the RLContext parameter is null when constructing the validator.</remarks>
public class AddUserToPlanProcedureCommandValidator : AbstractValidator<AddUserToPlanProcedureCommand>
{
    private readonly RLContext _context;
    /// <summary>
    /// Adds validation rules for the AddUserToPlanProcedureCommand, ensuring that PlanId, ProcedureId, and UserId are positive integers. It also includes asynchronous validation to check the existence of the specified plan-procedure association and user in the database context. An ArgumentNullException is thrown if the provided RLContext is null when initializing the validator. Use this class to validate commands before adding a user to a plan procedure to prevent invalid operations.
    /// </summary>
    /// <param name="context">context used to access the database for validation checks. This parameter must not be null.</param>
    /// <exception cref="ArgumentNullException"> throws when the provided RLContext is null.</exception>
    public AddUserToPlanProcedureCommandValidator(RLContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleFor(x => x.PlanId).GreaterThan(0).WithMessage("PlanId must be a positive integer.");
        RuleFor(x => x.ProcedureId).GreaterThan(0).WithMessage("ProcedureId must be a positive integer.");
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId must be a positive integer.");
        RuleFor(x=>x).MustAsync(PlanProcedureExists).WithMessage("The specified plan-procedure association does not exist.");
        RuleFor(x=>x.UserId).MustAsync(UserExists).WithMessage("The specified user does not exist.");
    }
    /// <summary>
    /// Checks if a plan-procedure association exists in the database for the given PlanId and ProcedureId from the command. This method is used as part of the validation process to ensure that the specified association is valid before attempting to add a user to it. It performs an asynchronous query against the PlanProcedures table in the RLContext to verify the existence of the association. If no matching record is found, the validation will fail with an appropriate error message.
    /// </summary>
    /// <param name="request">Add user to plan procedure command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns true if a plan-procedure association exists.</returns>
    private async Task<bool> PlanProcedureExists(AddUserToPlanProcedureCommand request, CancellationToken cancellationToken) => 
        await _context.PlanProcedures.AsNoTracking().AnyAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId, cancellationToken);
    /// <summary>
    /// Checks if a user exists in the database for the given UserId from the command. This method is used as part of the validation process to ensure that the specified user is valid before attempting to add them to a plan procedure. It performs an asynchronous query against the Users table in the RLContext to verify the existence of the user. If no matching record is found, the validation will fail with an appropriate error message.
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>Returns true if user exists</returns>
    private async Task<bool> UserExists(int userId, CancellationToken cancellationToken) => await _context.Users.AsNoTracking().AnyAsync(u => u.UserId == userId, cancellationToken);
     
}
