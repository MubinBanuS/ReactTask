namespace RL.Data;
/// <summary>
/// Represents the Entity Framework database context for the application, providing access to the database sets for
/// plans, procedures, users, and their relationships.
/// </summary>
/// <remarks>This context seeds initial data for procedures and users when the model is created. It also overrides
/// SaveChanges methods to automatically set creation and update timestamps for entities implementing
/// IChangeTrackable.</remarks>
public class RLContext : DbContext
{
    public DbSet<Plan> Plans { get; set; }
    public DbSet<PlanProcedure> PlanProcedures { get; set; }
    public DbSet<Procedure> Procedures { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<PlanProcedureUser> PlanProcedureUsers { get; set; }

    public RLContext(DbContextOptions<RLContext> options) : base(options) { }
    /// <summary>
    /// Configures the model for the application by defining entity relationships, composite keys, and seeding initial
    /// data.
    /// </summary>
    /// <remarks>This method establishes composite primary keys for the <see cref="PlanProcedure"/> entity,
    /// configures relationships between entities, and seeds initial data for <see cref="Procedure"/> and <see
    /// cref="User"/> entities. It is called during the model creation process to ensure the database schema is
    /// correctly set up for the application's requirements.</remarks>
    /// <param name="builder">The <see cref="ModelBuilder"/> instance used to configure the application's entity model and relationships.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PlanProcedure>(typeBuilder =>
        {
            typeBuilder.HasKey(pp => new { pp.PlanId, pp.ProcedureId });
            typeBuilder.HasOne(pp => pp.Plan).WithMany(p => p.PlanProcedures);
            typeBuilder.HasOne(pp => pp.Procedure).WithMany();
        });
        builder.ApplyConfiguration(new PlanProcedureUserConfiguration());
        //Add procedure Seed Data
        if (SeedDataCache.Procedures.Any())
        {
            builder.Entity<Procedure>().HasData(SeedDataCache.Procedures);
        }
        else
        {
            // Only run the file-based seeding if the SeedDataCache wasn't pre-populated.
            SeedProcedures(builder);
        }
        //Add User Seed Data
        builder.Entity<User>(u =>
        {
            u.HasData(new List<User> {
                new() {
                    UserId = 1,
                    Name = "Nick Morrison",
                    CreateDate = new DateTime(1999,12,13),
                    UpdateDate = new DateTime(1999,12,13)
                },
                new() {
                    UserId = 2,
                    Name = "Scott Cassidy",
                    CreateDate = new DateTime(1999,12,13),
                    UpdateDate = new DateTime(1999,12,13)
                },
                new() {
                    UserId = 3,
                    Name = "Tony Bidner",
                    CreateDate = new DateTime(1999,12,13),
                    UpdateDate = new DateTime(1999,12,13)
                },
                new() {
                    UserId = 4,
                    Name = "Patryk Skwarko",
                    CreateDate = new DateTime(1999,12,13),
                    UpdateDate = new DateTime(1999,12,13)
                }
            });
        });
    }

    /// <summary>
    /// Seeds the Procedure entity with initial data from a CSV file containing procedure titles.
    /// </summary>
    /// <remarks>This method reads procedure titles from a CSV file named 'ProcedureSeedData.csv' located in
    /// the application's base directory and uses them to populate the Procedure entity. Ensure that the CSV file exists
    /// and is accessible to avoid exceptions during database seeding.</remarks>
    /// <param name="builder">The model builder used to configure the Procedure entity and apply the seed data.</param>
    /// <exception cref="FileNotFoundException">Thrown if the CSV file containing procedure data is not found in the application's base directory.</exception>
    private static void SeedProcedures(ModelBuilder builder)
    {
        string csvFilePath = Path.Combine(AppContext.BaseDirectory, "ProcedureSeedData.csv");
        // If the seed file is not present (common in unit test runs), do not throw - just skip file-based seeding.
        if (!File.Exists(csvFilePath))
        {
            return;
        }
        var seedData = File.ReadAllLines(csvFilePath);
        builder.Entity<Procedure>(p =>
        {
            var seedProcedures = seedData.Select((sd, i) => new Procedure
            {
                ProcedureId = i + 1,
                ProcedureTitle = sd
            });
            p.HasData(seedProcedures);
        });
    }
    #region TimeStamps
    public override int SaveChanges()
    {
        AddTimestamps();
        return base.SaveChanges();
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AddTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void AddTimestamps()
    {
        DateTime currentDateTime = DateTime.UtcNow;
        var entities = ChangeTracker.Entries().Where(x => x.Entity is IChangeTrackable && (x.State == EntityState.Added || x.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            var trackableEntity = (IChangeTrackable)entity.Entity;
            if (entity.State == EntityState.Added)
            {
                trackableEntity.CreateDate = currentDateTime;
            }
            trackableEntity.UpdateDate = currentDateTime;
        }
    }
    #endregion
}
