namespace RL.Backend;
/// <summary>
/// StaticKeys is a static class that contains constant string values used throughout the application for configuration and settings. These keys include DefaultConnectionString, ReactAppBaseUrl, and AllowLocal, which are likely used for database connection strings, base URLs for the React application, and flags for allowing local development or testing. By centralizing these keys in a static class, it promotes consistency and maintainability across the codebase, making it easier to manage and update configuration values as needed.
/// </summary>
public static class StaticKeys
{
    public const string DefaultConnectionString = "Default";
    public const string ReactAppBaseUrl = "ReactAppBaseUrl";
    public const string AllowLocal = "AllowLocal";
}
