namespace RL.Data;
/// <summary>
/// Provides functionality for loading a collection of procedure data from a CSV seed file.
/// </summary>
public static class ProcedureSeedProvider
{
    /// <summary>
    /// Loads a collection of procedures from a CSV file located in the application's base directory. Each procedure is
    /// assigned a unique identifier and title based on the file contents.
    /// </summary>
    /// <remarks>The CSV file must contain one procedure title per line. Procedure identifiers are assigned
    /// sequentially starting from 1, based on the order of entries in the file.</remarks>
    /// <returns>An IReadOnlyList of Procedure objects, each representing a procedure loaded from the CSV file.</returns>
    public static IReadOnlyList<Procedure> LoadProcedures()
    {
        string csvFilePath = Path.Combine(AppContext.BaseDirectory, "ProcedureSeedData.csv");
        if (!File.Exists(csvFilePath))
        {
            // Return an empty list when the seed file is not present (makes tests resilient).
            return new List<Procedure>();
        }
        return File.ReadAllLines(csvFilePath)
            .Select((sd, i) => new Procedure
            {
                ProcedureId = i + 1,
                ProcedureTitle = sd
            }).ToList();
    }
}
