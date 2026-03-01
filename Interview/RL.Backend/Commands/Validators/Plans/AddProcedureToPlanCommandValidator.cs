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
    public AddProcedureToPlanCommandValidator(RLContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        RuleFor(x => x.PlanId).GreaterThan(0).WithMessage("PlanId must be a positive integer.");
        RuleFor(x => x.ProcedureId).GreaterThan(0).WithMessage("ProcedureId must be a positive integer.");
        RuleFor(x => x.PlanId).MustAsync(PlanExists).WithMessage("The specified plan does not exist.");
        RuleFor(x => x.ProcedureId).MustAsync(ProcedureExists).WithMessage("The specified procedure does not exist.");
    }
    private async Task<bool> PlanExists(int planId, CancellationToken cancellationToken) => await _context.Plans.AsNoTracking().AnyAsync(p => p.PlanId == planId, cancellationToken);
    private async Task<bool> ProcedureExists(int procedureId, CancellationToken cancellationToken) => await _context.Procedures.AsNoTracking().AnyAsync(p => p.ProcedureId == procedureId, cancellationToken);

}
