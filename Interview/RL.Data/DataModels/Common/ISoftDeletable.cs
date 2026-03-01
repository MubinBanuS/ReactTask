namespace RL.Data.DataModels.Common;
/// <summary>
/// Defines a contract for entities that support soft deletion, allowing them to be marked as deleted without being
/// removed from persistent storage.
/// </summary>
/// <remarks>Implementing this interface enables tracking of soft-deleted entities, which can be useful for
/// maintaining historical data or supporting restore operations. Entities implementing this interface typically use the
/// IsDeleted property to indicate deletion status and the DeletedAt property to record the time of deletion.</remarks>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets or sets a value indicating whether the item is marked as deleted.
    /// </summary>
    /// <remarks>This property is typically used to implement soft deletion, allowing items to be excluded
    /// from queries or operations without being physically removed from the data store.</remarks>
    bool IsDeleted { get; set; }
    /// <summary>
    /// Gets or sets the date and time when the entity was deleted. If the entity has not been deleted, this property is
    /// null.
    /// </summary>
    /// <remarks>A non-null value indicates that the entity has been marked as deleted. This property is
    /// commonly used to implement soft deletion, allowing entities to be excluded from active queries without being
    /// permanently removed from the data store.</remarks>
    DateTime? DeletedAt { get; set; }
}
