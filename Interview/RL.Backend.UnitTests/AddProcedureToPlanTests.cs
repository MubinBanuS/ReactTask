namespace RL.Backend.UnitTests;

[TestClass]
public class AddProcedureToPlanTests
{
    private RLContext context = null!;
    private AddProcedureToPlanCommandHandler procedureToPlanCommandHandler = null!;

    [TestInitialize]
    public void Setup()
    {
        context = DbContextHelper.CreateContext();
        var loggerMock = new Mock<ILogger<AddProcedureToPlanCommandHandler>>();
        procedureToPlanCommandHandler = new AddProcedureToPlanCommandHandler(context, loggerMock.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        context?.Dispose();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task AddProcedureToPlanTests_InvalidPlanId_ReturnsBadRequest(int planId)
    {
        var request = new AddProcedureToPlanCommand()
        {
            PlanId = planId,
            ProcedureId = 1
        };

        var result = await procedureToPlanCommandHandler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task AddProcedureToPlanTests_InvalidProcedureId_ReturnsBadRequest(int procedureId)
    {
        var request = new AddProcedureToPlanCommand()
        {
            PlanId = 1,
            ProcedureId = procedureId
        };

        var result = await procedureToPlanCommandHandler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AddProcedureToPlanTests_PlanIdNotFound_ReturnsNotFound(int planId)
    {
        var request = new AddProcedureToPlanCommand()
        {
            PlanId = planId,
            ProcedureId = 1
        };

        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = planId + 1
        });
        await context.SaveChangesAsync();

        var result = await procedureToPlanCommandHandler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<NotFoundException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AddProcedureToPlanTests_ProcedureIdNotFound_ReturnsNotFound(int procedureId)
    {
        var request = new AddProcedureToPlanCommand()
        {
            PlanId = 1,
            ProcedureId = procedureId
        };

        // Ensure the plan referenced by the request exists
        context.Plans.Add(new Data.DataModels.Plan
        {
            PlanId = 1
        });

        // Add a procedure with a different id so the requested procedure is missing
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId + 1,
            ProcedureTitle = "Test Procedure"
        });
        await context.SaveChangesAsync();

        var result = await procedureToPlanCommandHandler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<NotFoundException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(19, 1010)]
    [DataRow(35, 69)]
    public async Task AddProcedureToPlanTests_AlreadyContainsProcedure_ReturnsSuccess(int planId, int procedureId)
    {
        var request = new AddProcedureToPlanCommand()
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test Procedure" });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { ProcedureId = procedureId, PlanId = planId });
        await context.SaveChangesAsync();

        var result = await procedureToPlanCommandHandler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 1)]
    [DataRow(19, 1010)]
    [DataRow(35, 69)]
    public async Task AddProcedureToPlanTests_DoesntContainsProcedure_ReturnsSuccess(int planId, int procedureId)
    {
        var request = new AddProcedureToPlanCommand()
        {
            PlanId = planId,
            ProcedureId = procedureId
        };

        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test Procedure" });
        await context.SaveChangesAsync();

        var result = await procedureToPlanCommandHandler.Handle(request, CancellationToken.None);

        var dbPlanProcedure = await context.PlanProcedures.FirstOrDefaultAsync(pp => pp.PlanId == planId && pp.ProcedureId == procedureId);
        dbPlanProcedure.Should().NotBeNull();

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();
    }
}