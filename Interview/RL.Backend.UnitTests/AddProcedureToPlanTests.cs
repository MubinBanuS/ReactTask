namespace RL.Backend.UnitTests;

[TestClass]
public class AddProcedureToPlanTests
{
    private RLContext context = null!;
    private AddProcedureToPlanCommandHandler handler = null!;

    [TestInitialize]
    public void Setup()
    {
        context = DbContextHelper.CreateContext();
        var loggerMock = new Mock<ILogger<AddProcedureToPlanCommandHandler>>();
        handler = new AddProcedureToPlanCommandHandler(context, loggerMock.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        context?.Dispose();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(int.MinValue)]
    public async Task AddProcedureToPlan_InvalidPlanId_AddsPlanProcedure(int planId)
    {
        var request = new AddProcedureToPlanCommand
        {
            PlanId = planId,
            ProcedureId = 1
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var dbEntry = await context.PlanProcedures.FirstOrDefaultAsync(pp =>
            pp.PlanId == planId && pp.ProcedureId == 1);
        dbEntry.Should().NotBeNull();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(int.MinValue)]
    public async Task AddProcedureToPlan_InvalidProcedureId_AddsPlanProcedure(int procedureId)
    {
        var request = new AddProcedureToPlanCommand
        {
            PlanId = 1,
            ProcedureId = procedureId
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var dbEntry = await context.PlanProcedures.FirstOrDefaultAsync(pp =>
            pp.PlanId == 1 && pp.ProcedureId == procedureId);
        dbEntry.Should().NotBeNull();
    }

    [TestMethod]
    public async Task AddProcedureToPlan_PlanIdNotFound_AddsPlanProcedure()
    {
        var request = new AddProcedureToPlanCommand
        {
            PlanId = 77,
            ProcedureId = 1
        };

        // ensure DB doesn't contain the requested plan
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 78 });
        await context.SaveChangesAsync();

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var dbEntry = await context.PlanProcedures.FirstOrDefaultAsync(pp =>
            pp.PlanId == 77 && pp.ProcedureId == 1);
        dbEntry.Should().NotBeNull();
    }

    [TestMethod]
    public async Task AddProcedureToPlan_ProcedureIdNotFound_AddsPlanProcedure()
    {
        var request = new AddProcedureToPlanCommand
        {
            PlanId = 1,
            ProcedureId = 9999
        };

        // ensure plan exists but procedure does not
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 10000, ProcedureTitle = "Other" });
        await context.SaveChangesAsync();

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var dbEntry = await context.PlanProcedures.FirstOrDefaultAsync(pp =>
            pp.PlanId == 1 && pp.ProcedureId == 9999);
        dbEntry.Should().NotBeNull();
    }

    [TestMethod]
    public async Task AddProcedureToPlan_AlreadyContainsProcedure_ReturnsSuccess_NoDuplicate()
    {
        var planId = 5;
        var procedureId = 10;

        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test" });

        // existing association
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = planId, ProcedureId = procedureId });
        await context.SaveChangesAsync();

        var request = new AddProcedureToPlanCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var count = await context.PlanProcedures.CountAsync(pp => pp.PlanId == planId && pp.ProcedureId == procedureId);
        count.Should().Be(1);
    }

    [TestMethod]
    public async Task AddProcedureToPlan_DoesntContainProcedure_ReturnsSuccess_Added()
    {
        var planId = 6;
        var procedureId = 11;

        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test" });
        await context.SaveChangesAsync();

        var request = new AddProcedureToPlanCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var dbEntry = await context.PlanProcedures.FirstOrDefaultAsync(pp => pp.PlanId == planId && pp.ProcedureId == procedureId);
        dbEntry.Should().NotBeNull();
    }

    [TestMethod]
    public async Task Validator_NegativeIds_ReturnsValidationErrors()
    {
        var validator = new AddProcedureToPlanCommandValidator(context);

        var invalidPlan = new AddProcedureToPlanCommand { PlanId = -1, ProcedureId = 1 };
        var resultPlan = await validator.ValidateAsync(invalidPlan);
        resultPlan.IsValid.Should().BeFalse();
        resultPlan.Errors.Should().Contain(e => e.PropertyName == nameof(invalidPlan.PlanId) && e.ErrorMessage == "PlanId must be a positive integer.");

        var invalidProcedure = new AddProcedureToPlanCommand { PlanId = 1, ProcedureId = 0 };
        var resultProcedure = await validator.ValidateAsync(invalidProcedure);
        resultProcedure.IsValid.Should().BeFalse();
        resultProcedure.Errors.Should().Contain(e => e.PropertyName == nameof(invalidProcedure.ProcedureId) && e.ErrorMessage == "ProcedureId must be a positive integer.");
    }

    [TestMethod]
    public async Task Validator_PlanNotFound_ReturnsValidationError()
    {
        var validator = new AddProcedureToPlanCommandValidator(context);

        // ensure plan not present
        var command = new AddProcedureToPlanCommand { PlanId = 5000, ProcedureId = 1 };

        var result = await validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.PlanId) && e.ErrorMessage == "The specified plan does not exist.");
    }

    [TestMethod]
    public async Task Validator_ProcedureNotFound_ReturnsValidationError()
    {
        var validator = new AddProcedureToPlanCommandValidator(context);

        // ensure procedure not present
        var command = new AddProcedureToPlanCommand { PlanId = 1, ProcedureId = 6000 };

        var result = await validator.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.ProcedureId) && e.ErrorMessage == "The specified procedure does not exist.");
    }

    [TestMethod]
    public async Task Validator_ValidCommand_Passes()
    {
        // add plan and procedure so validator's async existence checks succeed
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 200 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 300, ProcedureTitle = "Seed" });
        await context.SaveChangesAsync();

        var validator = new AddProcedureToPlanCommandValidator(context);
        var command = new AddProcedureToPlanCommand { PlanId = 200, ProcedureId = 300 };

        var result = await validator.ValidateAsync(command);
        result.IsValid.Should().BeTrue();
    }
}