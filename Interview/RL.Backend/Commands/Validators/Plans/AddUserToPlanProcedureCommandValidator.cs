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
    public AddUserToPlanProcedureCommandValidator(RLContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleFor(x => x.PlanId).GreaterThan(0).WithMessage("PlanId must be a positive integer.");
        RuleFor(x => x.ProcedureId).GreaterThan(0).WithMessage("ProcedureId must be a positive integer.");
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId must be a positive integer.");
        RuleFor(x=>x).MustAsync(PlanProcedureExists).WithMessage("The specified plan-procedure association does not exist.");
        RuleFor(x=>x.UserId).MustAsync(UserExists).WithMessage("The specified user does not exist.");
    }
    private async Task<bool> PlanProcedureExists(AddUserToPlanProcedureCommand request, CancellationToken cancellationToken) => await _context.PlanProcedures.AsNoTracking().AnyAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId, cancellationToken);
    private async Task<bool> UserExists(int userId, CancellationToken cancellationToken) => await _context.Users.AsNoTracking().AnyAsync(u => u.UserId == userId, cancellationToken);
     
}
