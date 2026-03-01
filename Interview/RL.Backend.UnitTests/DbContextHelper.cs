namespace RL.Backend.UnitTests;

public static class DbContextHelper
{
    public static RLContext CreateContext(string name = "")
    {
        if (string.IsNullOrWhiteSpace(name))
            name = Guid.NewGuid().ToString();

        //Loads procedure data
        SeedDataCache.Procedures = ProcedureSeedProvider.LoadProcedures();

        var dbContextOptions = new DbContextOptionsBuilder<RLContext>()
            .UseInMemoryDatabase(databaseName: name)
            .Options;

        return new RLContext(dbContextOptions);
    }
}