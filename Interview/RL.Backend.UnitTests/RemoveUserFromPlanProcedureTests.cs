namespace RL.Backend.UnitTests;

[TestClass]
public class RemoveUserFromPlanProcedureTests
{
    private RLContext context = null!;
    private RemoveUserFromPlanProcedureCommandHandler handler = null!;

    [TestInitialize]
    public void Setup()
    {
        context = DbContextHelper.CreateContext();
        var loggerMock = new Mock<ILogger<RemoveUserFromPlanProcedureCommandHandler>>();
        handler = new RemoveUserFromPlanProcedureCommandHandler(context, loggerMock.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        context?.Dispose();
    }

    [TestMethod]
    public async Task RemoveUserFromPlanProcedure_RemoveSpecificUser_MarksAsDeleted()
    {
        var planId = 1;
        var procedureId = 1;
        var userId = 1;

        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test" });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = planId, ProcedureId = procedureId });
        context.Users.Add(new Data.DataModels.User { UserId = userId });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = userId });
        await context.SaveChangesAsync();

        var request = new RemoveUserFromPlanProcedureCommand { PlanId = planId, ProcedureId = procedureId, UserId = userId };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Succeeded.Should().BeTrue();

        var entry = await context.PlanProcedureUsers.IgnoreQueryFilters().FirstOrDefaultAsync(ppu => ppu.PlanId == planId && ppu.ProcedureId == procedureId && ppu.UserId == userId);
        entry.Should().NotBeNull();
        entry.IsDeleted.Should().BeTrue();
        entry.DeletedAt.Should().NotBeNull();
    }


    [TestMethod]
    public async Task RemoveUserFromPlanProcedure_RemoveNonExistingUser_ReturnsSuccess_NoChange()
    {
        var planId = 3;
        var procedureId = 3;

        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test" });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = planId, ProcedureId = procedureId });
        // existing mapping for user 20
        context.Users.Add(new Data.DataModels.User { UserId = 20 });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = 20 });
        await context.SaveChangesAsync();

        var request = new RemoveUserFromPlanProcedureCommand { PlanId = planId, ProcedureId = procedureId, UserId = 999 }; // not present

        var result = await handler.Handle(request, CancellationToken.None);

        result.Succeeded.Should().BeTrue();

        // ensure existing mapping still present and not deleted
        var entry = await context.PlanProcedureUsers.IgnoreQueryFilters().FirstOrDefaultAsync(ppu => ppu.PlanId == planId && ppu.ProcedureId == procedureId && ppu.UserId == 20);
        entry.Should().NotBeNull();
        entry.IsDeleted.Should().BeFalse();
    }

    [TestMethod]
    public async Task RemoveUserFromPlanProcedure_RemoveAllWhenNoUsers_ReturnsSuccess()
    {
        var planId = 4;
        var procedureId = 4;

        // no PlanProcedureUsers added
        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test" });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = planId, ProcedureId = procedureId });
        await context.SaveChangesAsync();

        var request = new RemoveUserFromPlanProcedureCommand { PlanId = planId, ProcedureId = procedureId, UserId = 0 };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
    }
}
