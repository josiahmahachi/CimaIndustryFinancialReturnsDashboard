using Microsoft.AspNetCore.Components.Forms;

namespace blazor_wasm_ui.Models;

/// <summary>
/// Data model for extension request form submission
/// Mirrors React ExtensionRequestData interface structure
/// </summary>
public class ExtensionRequestData
{
    /// <summary>
    /// Type of extension being requested (e.g., "2-month", "3-month", "waiver")
    /// </summary>
    public string ExtensionType { get; set; }

    /// <summary>
    /// Reason for waiver if extension type is waiver
    /// </summary>
    public string? WaiverReason { get; set; }

    /// <summary>
    /// List of sub-funds for multi-fund structures
    /// </summary>
    public List<SubFund>? SubFunds { get; set; }

    /// <summary>
    /// Cover letter file
    /// </summary>
    public IBrowserFile? CoverLetter { get; set; }

    /// <summary>
    /// Auditor letter file
    /// </summary>
    public IBrowserFile? AuditorLetter { get; set; }

    /// <summary>
    /// Operator affidavit file
    /// </summary>
    public IBrowserFile? OperatorAffidavit { get; set; }

    /// <summary>
    /// Administrator letter file
    /// </summary>
    public IBrowserFile? AdministratorLetter { get; set; }

    /// <summary>
    /// Liquidator report file
    /// </summary>
    public IBrowserFile? LiquidatorReport { get; set; }

    /// <summary>
    /// Other supporting documents file
    /// </summary>
    public IBrowserFile? OtherDocuments { get; set; }

    /// <summary>
    /// Additional comments from the user
    /// </summary>
    public string? AdditionalComments { get; set; }
}
