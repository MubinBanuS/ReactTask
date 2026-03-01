namespace RL.Data;
/// <summary>
/// Provides a static cache for seed data, specifically a list of procedures.
/// </summary>
/// <remarks>This class holds a read-only list of procedures that can be accessed throughout the application. The
/// list can be populated with seed data at application startup to ensure consistent access to predefined
/// procedures.</remarks>
public static class SeedDataCache
{
    /// <summary>
    /// Gets or sets the collection of procedures available in the application.
    /// </summary>
    /// <remarks>The list of procedures is exposed as a read-only collection to consumers, ensuring that
    /// procedures cannot be modified externally. The property can be updated internally to reflect changes in the
    /// available procedures as needed.</remarks>
    public static IReadOnlyList<Procedure> Procedures { get; set; } = new List<Procedure>();
}
