using Microsoft.AspNetCore.Components.Forms;

namespace blazor_wasm_ui.Models;

/// <summary>
/// Represents document upload data for extension requests.
/// Contains file references and supporting comments.
/// </summary>
public class DocumentUploadData
{
    /// <summary>
    /// Cover letter document (required for all requests).
    /// </summary>
    public IBrowserFile? CoverLetter { get; set; }

    /// <summary>
    /// Auditor letter (required for 2-3 month extensions).
    /// </summary>
    public IBrowserFile? AuditorLetter { get; set; }

    /// <summary>
    /// Operator affidavit (required for specific waiver reasons).
    /// </summary>
    public IBrowserFile? OperatorAffidavit { get; set; }

    /// <summary>
    /// Administrator letter for deregistered funds.
    /// </summary>
    public IBrowserFile? AdministratorLetter { get; set; }

    /// <summary>
    /// Liquidator report for liquidated funds.
    /// </summary>
    public IBrowserFile? LiquidatorReport { get; set; }

    /// <summary>
    /// Any other supporting documents (optional).
    /// </summary>
    public IBrowserFile? OtherDocuments { get; set; }

    /// <summary>
    /// Additional comments or explanations (max 5000 characters).
    /// </summary>
    public string? AdditionalComments { get; set; }

    /// <summary>
    /// Indicates whether all required documents have been provided.
    /// </summary>
    public bool IsComplete =>
        CoverLetter != null;
}
