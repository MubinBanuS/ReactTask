namespace RL.Data.DataModels;

public class PlanProcedureUser : IChangeTrackable, ISoftDeletable
{
    public int PlanId { get; set; }
    public int ProcedureId { get; set; }
    public int UserId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public virtual User User { get; set; }
    public virtual PlanProcedure PlanProcedure { get; set; }

}
