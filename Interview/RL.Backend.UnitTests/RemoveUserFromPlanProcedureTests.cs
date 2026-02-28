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
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task RemoveUserFromPlanProcedure_InvalidPlanId_ReturnsBadRequest(int planId)
    {
        var request = new RemoveUserFromPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = 1,
            UserId = null
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task RemoveUserFromPlanProcedure_InvalidProcedureId_ReturnsBadRequest(int procedureId)
    {
        var request = new RemoveUserFromPlanProcedureCommand
        {
            PlanId = 1,
            ProcedureId = procedureId,
            UserId = null
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(int.MinValue)]
    public async Task RemoveUserFromPlanProcedure_NegativeUserId_ReturnsBadRequest(int userId)
    {
        var request = new RemoveUserFromPlanProcedureCommand
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = userId
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task RemoveUserFromPlanProcedure_RemoveAll_NoAssociations_ReturnsSuccess()
    {
        var request = new RemoveUserFromPlanProcedureCommand
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = 0 // remove all users; none exist
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    public async Task RemoveUserFromPlanProcedure_RemoveAll_WithAssociations_RemovesAndReturnsSuccess()
    {
        var planId = 1;
        var procedureId = 1;

        // add an association to remove
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = 5 });
        await context.SaveChangesAsync();

        var request = new RemoveUserFromPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = null // remove all users for plan-procedure
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var remaining = await context.PlanProcedureUsers.AnyAsync(p => p.PlanId == planId && p.ProcedureId == procedureId);
        remaining.Should().BeFalse();
    }

    [TestMethod]
    public async Task RemoveUserFromPlanProcedure_RemoveSpecific_AssociationNotFound_ReturnsNotFound()
    {
        var request = new RemoveUserFromPlanProcedureCommand
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = 999 // no such association
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<NotFoundException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    public async Task RemoveUserFromPlanProcedure_RemoveSpecific_AssociationExists_RemovesAndReturnsSuccess()
    {
        var planId = 2;
        var procedureId = 2;
        var userId = 10;

        // create the association to be removed
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = userId });
        await context.SaveChangesAsync();

        var request = new RemoveUserFromPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var exists = await context.PlanProcedureUsers.AnyAsync(p => p.PlanId == planId && p.ProcedureId == procedureId && p.UserId == userId);
        exists.Should().BeFalse();
    }
}