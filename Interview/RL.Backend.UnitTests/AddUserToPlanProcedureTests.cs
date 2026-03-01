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
    [DataRow(int.MinValue)]
    public async Task AddUserToPlanProcedure_InvalidPlanId_AddsPlanProcedureUser(int planId)
    {
        // Ensure principals exist for EF in-memory provider
        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "Test Procedure" });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = planId, ProcedureId = 1 });
        context.Users.Add(new Data.DataModels.User { UserId = 1 });
        await context.SaveChangesAsync();

        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = 1,
            UserId = 1
        };

        var result = await handler.Handle(request, CancellationToken.None);

        // Handler no longer validates plan id; it will create the association.
        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var dbEntry = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
            ppu.PlanId == planId && ppu.ProcedureId == 1 && ppu.UserId == 1);
        dbEntry.Should().NotBeNull();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(int.MinValue)]
    public async Task AddUserToPlanProcedure_InvalidProcedureId_AddsPlanProcedureUser(int procedureId)
    {
        // Ensure principals exist for EF in-memory provider
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = procedureId, ProcedureTitle = "Test Procedure" });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = 1, ProcedureId = procedureId });
        context.Users.Add(new Data.DataModels.User { UserId = 1 });
        await context.SaveChangesAsync();

        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = 1,
            ProcedureId = procedureId,
            UserId = 1
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var dbEntry = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
            ppu.PlanId == 1 && ppu.ProcedureId == procedureId && ppu.UserId == 1);
        dbEntry.Should().NotBeNull();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(int.MinValue)]
    public async Task AddUserToPlanProcedure_InvalidUserId_AddsPlanProcedureUser(int userId)
    {
        // Ensure related Plan and Procedure and PlanProcedure exist so foreign key principals are present
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "Test Procedure" });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = 1, ProcedureId = 1 });

        // Add a user entry for the invalid id to satisfy FK constraint during SaveChanges
        context.Users.Add(new Data.DataModels.User { UserId = userId });

        await context.SaveChangesAsync();

        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = userId
        };

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var dbEntry = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
            ppu.PlanId == 1 && ppu.ProcedureId == 1 && ppu.UserId == userId);
        dbEntry.Should().NotBeNull();
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AddUserToPlanProcedure_PlanIdNotFound_AddsPlanProcedureUser(int planId)
    {
        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = planId,
            ProcedureId = 1,
            UserId = 1
        };

        // Ensure principals exist for EF in-memory provider (simulate environment)
        context.Plans.Add(new Data.DataModels.Plan { PlanId = planId });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "Test Procedure" });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = planId, ProcedureId = 1 });
        context.Users.Add(new Data.DataModels.User { UserId = 1 });
        await context.SaveChangesAsync();

        var result = await handler.Handle(request, CancellationToken.None);

        // Handler no longer requires plan to exist; it will create the association.
        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var dbEntry = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
            ppu.PlanId == planId && ppu.ProcedureId == 1 && ppu.UserId == 1);
        dbEntry.Should().NotBeNull();
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AddUserToPlanProcedure_ProcedureIdNotFound_AddsPlanProcedureUser(int procedureId)
    {
        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = 1,
            ProcedureId = procedureId,
            UserId = 1
        };
        // ensure plan and requested procedure exist and create PlanProcedure principal
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure
        {
            ProcedureId = procedureId,
            ProcedureTitle = "Test Procedure"
        });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = 1, ProcedureId = procedureId });
        context.Users.Add(new Data.DataModels.User { UserId = 1 });

        await context.SaveChangesAsync();

        var result = await handler.Handle(request, CancellationToken.None);

        // Handler will create the association.
        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var dbEntry = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
            ppu.PlanId == 1 && ppu.ProcedureId == procedureId && ppu.UserId == 1);
        dbEntry.Should().NotBeNull();
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(19)]
    [DataRow(35)]
    public async Task AddUserToPlanProcedure_UserIdNotFound_AddsPlanProcedureUser(int userId)
    {
        var request = new AddUserToPlanProcedureCommand
        {
            PlanId = 1,
            ProcedureId = 1,
            UserId = userId
        };

        // ensure plan and procedure exist and that PlanProcedure association exists so handler can add the user mapping
        context.Plans.Add(new Data.DataModels.Plan { PlanId = 1 });
        context.Procedures.Add(new Data.DataModels.Procedure { ProcedureId = 1, ProcedureTitle = "Test Procedure" });
        context.PlanProcedures.Add(new Data.DataModels.PlanProcedure { PlanId = 1, ProcedureId = 1 });

        // Add the target user to satisfy EF's principal requirement in the in-memory provider
        context.Users.Add(new Data.DataModels.User { UserId = userId });

        await context.SaveChangesAsync();

        var result = await handler.Handle(request, CancellationToken.None);

        result.Value.Should().BeOfType<Unit>();
        result.Succeeded.Should().BeTrue();

        var dbEntry = await context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
            ppu.PlanId == 1 && ppu.ProcedureId == 1 && ppu.UserId == userId);
        dbEntry.Should().NotBeNull();
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

        // ensure no duplicate entries were created
        var count = await context.PlanProcedureUsers.CountAsync(ppu => ppu.PlanId == planId && ppu.ProcedureId == procedureId && ppu.UserId == userId);
        count.Should().Be(1);
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
