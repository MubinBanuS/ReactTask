namespace RL.Backend.UnitTests;

[TestClass]
public class AddUserToPlanProcedureTests
{
    private RLContext context = null!;
    private AddUserToPlanProcedureCommandHandler handler = null!;

    [TestInitialize]
    public void Setup()
    {
        context = DbContextHelper.CreateContext();
        var loggerMock = new Mock<ILogger<AddUserToPlanProcedureCommandHandler>>();
        handler = new AddUserToPlanProcedureCommandHandler(context, loggerMock.Object);
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
    public async Task AddUserToPlanProcedure_InvalidPlanId_ReturnsBadRequest(int planId)
    {
        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = 1,
            UserId = 1
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task AddUserToPlanProcedure_InvalidProcedureId_ReturnsBadRequest(int procedureId)
    {
        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = 1,
            ProcedureId = procedureId,
            UserId = 1
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task AddUserToPlanProcedure_InvalidUserId_ReturnsBadRequest(int userId)
    {
        var request = new AddUserToPlanProcedureCommand
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
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AddUserToPlanProcedure_PlanIdNotFound_ReturnsBadRequest(int planId)
    {
        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = 1,
            UserId = 1
        };

        // add a different plan so requested plan is missing (no PlanProcedure created)
        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId + 1 });
        await context.SaveChangesAsync();

        var result = await handler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AddUserToPlanProcedure_ProcedureIdNotFound_ReturnsBadRequest(int procedureId)
    {
        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = 1,
            ProcedureId = procedureId,
            UserId = 1
        };

        // ensure plan exists
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });

        // add a different procedure so requested one is missing (no PlanProcedure created)
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId + 1,
            ProcedureTitle = "Test Procedure"
        });

        await context.SaveChangesAsync();

        var result = await handler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AddUserToPlanProcedure_UserIdNotFound_ReturnsBadRequest(int userId)
    {
        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = userId
        };

        // ensure plan and procedure exist and that PlanProcedure association exists so validation reaches user check
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "Test Procedure" });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = 1, ProcedureId = 1 });

        // add a different user so requested user is missing
        context.Users.Add(new Data.DataModels.User { UserId = userId + 1 });

        await context.SaveChangesAsync();

        var result = await handler.Handle(request, CancellationToken.None);

        result.Exception.Should().BeOfType<BadRequestException>();
        result.Succeeded.Should().BeFalse();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(19, 1010, 77)]
    [DataRow(35, 69, 42)]
    public async Task AddUserToPlanProcedure_AlreadyContainsUser_ReturnsSuccess(int planId, int procedureId, int userId)
    {
        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test Procedure" });
        context.Users.Add(new Data.DataModels.User { UserId = userId });

        // ensure the plan-procedure exists and the user association already exists
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = planId, ProcedureId = procedureId });
        context.PlanProcedureUsers.Add(new Data.DataModels.PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = userId });

        await context.SaveChangesAsync();

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();
    }

    [TestMethod]
    [DataRow(1, 1, 1)]
    [DataRow(19, 1010, 77)]
    [DataRow(35, 69, 42)]
    public async Task AddUserToPlanProcedure_DoesntContainUser_ReturnsSuccess(int planId, int procedureId, int userId)
    {
        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test Procedure" });
        context.Users.Add(new Data.DataModels.User { UserId = userId });

        // ensure plan-procedure exists so handler can add the PlanProcedureUser
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = planId, ProcedureId = procedureId });

        await context.SaveChangesAsync();

        var result = await handler.Handle(request, CancellationToken.None);

        var dbEntry = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
            ppu.PlanId == planId && ppu.ProcedureId == procedureId && ppu.UserId == userId);
        dbEntry.Should().NotBeNull();

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();
    }
}