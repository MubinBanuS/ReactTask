namespace RL.Data.Configurations;

public class PlanProcedureUserConfiguration : IEntityTypeConfiguration<PlanProcedureUser>
{
    public void Configure(EntityTypeBuilder<PlanProcedureUser> builder)
    {
        // Composite key of PlanId, ProcedureId, and UserId to ensure uniqueness of each user assignment to a specific procedure within a plan
        builder.HasKey(ppu => new { ppu.PlanId, ppu.ProcedureId, ppu.UserId });

        // Relationships: Each PlanProcedureUser has one PlanProcedure and PlanProcedure can have many PlanProcedureUsers
        // Foreign key is composite of PlanId and ProcedureId in PlanProcedureUser referencing the same in PlanProcedure
        builder.HasOne(ppu => ppu.PlanProcedure).WithMany(pp => pp.PlanProcedureUsers).HasForeignKey(ppu => new { ppu.PlanId, ppu.ProcedureId });

        // Each PlanProcedureUser has one User and User can have many PlanProcedureUsers
        builder.HasOne(ppu => ppu.User).WithMany().HasForeignKey(ppu => ppu.UserId);

        // soft delete configuration 
        builder.Property(ppu => ppu.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasQueryFilter(ppu => !ppu.IsDeleted);

        // Performance optimization: Index on IsDeleted to speed up queries filtering by this column
        builder.HasIndex(ppu => ppu.IsDeleted);
    }
}
