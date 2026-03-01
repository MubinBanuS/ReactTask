namespace RL.Backend.Commands.Validators.Plans;
/// <summary>
/// Provides validation logic for the RemoveUserFromPlanProcedureCommand to ensure that command parameters meet required
/// criteria before processing.
/// </summary>
/// <remarks>This validator enforces that PlanId and ProcedureId are positive integers, and that UserId is either
/// null or a non-negative integer. It also verifies the existence of the specified plan-procedure association and, when
/// provided, the user. Use this class to validate commands prior to removing a user from a plan procedure to prevent
/// invalid operations.</remarks>
public class RemoveUserFromPlanProcedureCommandValidator : AbstractValidator<RemoveUserFromPlanProcedureCommand>
{
    private readonly RLContext _context;
    public RemoveUserFromPlanProcedureCommandValidator(RLContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleFor(x => x.PlanId).GreaterThan(0).WithMessage("PlanId must be a positive integer.");
        RuleFor(x => x.ProcedureId).GreaterThan(0).WithMessage("ProcedureId must be a positive integer.");
        RuleFor(x => x.UserId).Must(id => id is null or >= 0).WithMessage("UserId must be 0 or a positive integer.");
        RuleFor(x => x).MustAsync(PlanProcedureExists).WithMessage("The specified plan-procedure association does not exist.");
        RuleFor(x => x.UserId).MustAsync(UserExistsWhenProvided).WithMessage("The specified user does not exist.");
    }
    private async Task<bool> PlanProcedureExists(RemoveUserFromPlanProcedureCommand request, CancellationToken cancellationToken) => await _context.PlanProcedures.AsNoTracking().AnyAsync(pp => pp.PlanId == request.PlanId && pp.ProcedureId == request.ProcedureId, cancellationToken);
    private async Task<bool> UserExistsWhenProvided(int? userId, CancellationToken cancellationToken) => userId is null or 0  || await _context.Users.AsNoTracking().AnyAsync(u => u.UserId == userId, cancellationToken);
}
