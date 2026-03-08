namespace RL.Backend.UnitTests;

[TestClass]
public class RemoveAllUsersFromPlanProcedureTests
{
    private RLContext context = null!;
    private RemoveAllUsersFromPlanProcedureCommandHandler handler = null!;

    [TestInitialize]
    public void Setup()
    {
        context = DbContextHelper.CreateContext();
        var loggerMock = new Mock<ILogger<RemoveAllUsersFromPlanProcedureCommandHandler>>();
        handler = new RemoveAllUsersFromPlanProcedureCommandHandler(context, loggerMock.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        context?.Dispose();
    }

    [TestMethod]
    public async Task RemoveAllUsersFromPlanProcedure_WhenUsersExist_MarksAllAsDeleted()
    {
        var planId = 10;
        var procedureId = 10;

        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test" });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = planId, ProcedureId = procedureId });
        context.Users.Add(new Data.DataModels.User { UserId = 101 });
        context.Users.Add(new Data.DataModels.User { UserId = 102 });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = 101 });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = 102 });
        await context.SaveChangesAsync();

        var request = new RemoveAllUsersFromPlanProcedureCommand { PlanId = planId, ProcedureId = procedureId };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Succeeded.Should().BeTrue();

        var entries = await context.PlanProcedureUsers.IgnoreQueryFilters().Where(ppu => ppu.PlanId == planId && ppu.ProcedureId == procedureId).ToListAsync();
        entries.Should().HaveCount(2);
        entries.All(e => e.IsDeleted).Should().BeTrue();
        entries.All(e => e.DeletedAt != null).Should().BeTrue();
    }

    [TestMethod]
    public async Task RemoveAllUsersFromPlanProcedure_WhenNoUsers_ReturnsSuccess_NoChange()
    {
        var planId = 11;
        var procedureId = 11;

        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test" });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = planId, ProcedureId = procedureId });
        await context.SaveChangesAsync();

        var request = new RemoveAllUsersFromPlanProcedureCommand { PlanId = planId, ProcedureId = procedureId };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
    }
}
