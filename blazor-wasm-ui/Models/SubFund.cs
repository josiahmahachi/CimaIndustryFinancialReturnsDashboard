namespace blazor_wasm_ui.Models;

/// <summary>
/// Represents a sub-fund within a multi-fund structure during extension request
/// </summary>
public class SubFund
{
    /// <summary>
    /// Sub-fund identifier
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Sub-fund name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description of the sub-fund (e.g., "Sub-fund 1 of 3")
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Current filing date for the sub-fund
    /// </summary>
    public string CurrentFilingDate { get; set; }

    /// <summary>
    /// Extension type for this sub-fund (e.g., "2-month", "3-month", "waiver", "none")
    /// </summary>
    public string ExtensionType { get; set; }

    /// <summary>
    /// Waiver reason if extension type is waiver
    /// </summary>
    public string? WaiverReason { get; set; }

    /// <summary>
    /// Number of extensions already used
    /// </summary>
    public int ExtensionsUsed { get; set; }

    /// <summary>
    /// Total days used across all extensions
    /// </summary>
    public int UsedExtensionDays { get; set; }

    /// <summary>
    /// Maximum number of extensions allowed
    /// </summary>
    public int MaxExtensions { get; set; }

    /// <summary>
    /// Entity identifier for this sub-fund
    /// </summary>
    public string EntityId { get; set; }
}
