using blazor_wasm_ui.Models;

namespace blazor_wasm_ui.Services;

/// <summary>
/// Configuration service providing centralized access to business rules, fees, and validation constants.
/// This service eliminates duplication of configuration across components and forms.
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Gets the base fee per month for extensions.
    /// </summary>
    decimal BaseExtensionFeePerMonth { get; }

    /// <summary>
    /// Gets the maximum number of extensions allowed per fund.
    /// </summary>
    int MaxExtensionsPerFund { get; }

    /// <summary>
    /// Gets the maximum total extension days allowed per fund.
    /// </summary>
    int MaxExtensionDaysPerFund { get; }

    /// <summary>
    /// Gets the number of days added per extension type.
    /// </summary>
    Dictionary<ExtensionType, int> ExtensionDaysByType { get; }

    /// <summary>
    /// Gets information about all available extension types.
    /// </summary>
    IReadOnlyDictionary<ExtensionType, ExtensionTypeInfo> ExtensionTypes { get; }

    /// <summary>
    /// Gets information about all available waiver reasons.
    /// </summary>
    IReadOnlyDictionary<WaiverReason, WaiverReasonInfo> WaiverReasons { get; }

    /// <summary>
    /// Gets the document requirements for a specific extension type and optional waiver reason.
    /// </summary>
    DocumentRequirements GetDocumentRequirements(ExtensionType extensionType, WaiverReason? waiverReason = null);

    /// <summary>
    /// Calculates the fee for a single extension request.
    /// </summary>
    /// <param name="extensionType">The type of extension requested</param>
    /// <param name="subFundCount">Number of sub-funds (for multi-fund structures)</param>
    /// <returns>The calculated fee in the base currency</returns>
    decimal CalculateExtensionFee(ExtensionType extensionType, int subFundCount = 1);

    /// <summary>
    /// Validates if an extension can be requested based on current usage.
    /// </summary>
    /// <param name="extensionsUsed">Number of extensions already used</param>
    /// <param name="usedExtensionDays">Total days of extensions already used</param>
    /// <param name="requestedExtensionType">The extension type being requested</param>
    /// <returns>Validation result with any error message</returns>
    ValidationResult ValidateExtensionRequest(int extensionsUsed, int usedExtensionDays, ExtensionType requestedExtensionType);

    /// <summary>
    /// Gets the display label for an extension type.
    /// </summary>
    string GetExtensionTypeLabel(ExtensionType type);

    /// <summary>
    /// Gets the display label for a waiver reason.
    /// </summary>
    string GetWaiverReasonLabel(WaiverReason reason);

    /// <summary>
    /// Converts a string to ExtensionType enum.
    /// </summary>
    ExtensionType ParseExtensionType(string? value);

    /// <summary>
    /// Converts a string to WaiverReason enum.
    /// </summary>
    WaiverReason ParseWaiverReason(string? value);
}

/// <summary>
/// Information about an extension type.
/// </summary>
public class ExtensionTypeInfo
{
    public string Label { get; set; } = string.Empty;
    public int DaysAdded { get; set; }
    public decimal FeeMultiplier { get; set; } // Multiple of BaseExtensionFeePerMonth
    public bool RequiresWaiverReason { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Information about a waiver reason.
/// </summary>
public class WaiverReasonInfo
{
    public string Label { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Document requirements for a specific extension/waiver scenario.
/// </summary>
public class DocumentRequirements
{
    public bool CoverLetterRequired { get; set; } = true;
    public bool AuditorLetterRequired { get; set; }
    public bool OperatorAffidavitRequired { get; set; }
    public bool AdministratorLetterRequired { get; set; }
    public bool LiquidatorReportRequired { get; set; }
    public bool OtherDocumentsAllowed { get; set; } = true;

    /// <summary>
    /// Gets all required documents as a list of human-readable strings.
    /// </summary>
    public List<string> GetRequiredDocuments()
    {
        var required = new List<string>();
        if (CoverLetterRequired) required.Add("Cover Letter");
        if (AuditorLetterRequired) required.Add("Auditor Letter");
        if (OperatorAffidavitRequired) required.Add("Operator Affidavit");
        if (AdministratorLetterRequired) required.Add("Administrator Letter");
        if (LiquidatorReportRequired) required.Add("Liquidator Report");
        return required;
    }

    /// <summary>
    /// Gets all optional documents as a list of human-readable strings.
    /// </summary>
    public List<string> GetOptionalDocuments()
    {
        var optional = new List<string>();
        if (!AuditorLetterRequired) optional.Add("Auditor Letter (optional)");
        if (!OperatorAffidavitRequired) optional.Add("Operator Affidavit (optional)");
        if (!AdministratorLetterRequired) optional.Add("Administrator Letter (optional)");
        if (!LiquidatorReportRequired) optional.Add("Liquidator Report (optional)");
        if (OtherDocumentsAllowed) optional.Add("Other Documents (optional)");
        return optional;
    }
}

/// <summary>
/// Result of a validation operation.
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; } = true;
    public string? ErrorMessage { get; set; }

    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Failure(string message) => new() { IsValid = false, ErrorMessage = message };
}
