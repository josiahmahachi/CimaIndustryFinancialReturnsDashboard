namespace blazor_wasm_ui.Models;

/// <summary>
/// Financial return model
/// Derived strictly from React mockReturnsData fields
/// </summary>
public class FinancialReturn
{
    /// <summary>
    /// Filing identifier
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Entity/Fund identifier
    /// </summary>
    public string EntityId { get; set; }

    /// <summary>
    /// Fund name
    /// </summary>
    public string FundName { get; set; }

    /// <summary>
    /// Type of fund (Mutual or Private)
    /// </summary>
    public FundType FundType { get; set; }

    /// <summary>
    /// Fund structure (Single or Multi)
    /// </summary>
    public FundStructure? FundStructure { get; set; }

    /// <summary>
    /// Parent fund name (for multi-fund structures)
    /// </summary>
    public string? ParentFundName { get; set; }

    /// <summary>
    /// Reporting period (e.g., "Q4 2024")
    /// </summary>
    public string Period { get; set; }

    /// <summary>
    /// Human-readable period description (e.g., "Oct 1 - Dec 31, 2024")
    /// </summary>
    public string PeriodDescription { get; set; }

    /// <summary>
    /// Filing due date (ISO format)
    /// </summary>
    public string DueDate { get; set; }

    /// <summary>
    /// Human-readable due date description (e.g., "5 days remaining")
    /// </summary>
    public string DueDateDescription { get; set; }

    /// <summary>
    /// Current filing status
    /// </summary>
    public ReturnStatus Status { get; set; }

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
    /// Human-readable extension availability description
    /// </summary>
    public string ExtensionDescription { get; set; }

    /// <summary>
    /// Whether an extension can be requested for this return
    /// </summary>
    public bool CanRequestExtension { get; set; }

    /// <summary>
    /// Whether this return is marked as urgent
    /// </summary>
    public bool IsUrgent { get; set; }

    /// <summary>
    /// Type of pending request if any (extension, waiver, deferral)
    /// </summary>
    public PendingRequestType? PendingRequestType { get; set; }

    /// <summary>
    /// Status of pending request if any (pending, approved, rejected)
    /// </summary>
    public PendingRequestStatus? PendingRequestStatus { get; set; }
}
