using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using blazor_wasm_ui.Models;
using blazor_wasm_ui.Services;

namespace blazor_wasm_ui.Components;

/// <summary>
/// DocumentUpload component handles multi-document submission with file validation,
/// conditional document requirements based on extension type/waiver reason, and
/// support for additional comments. Implements client-side file validation.
/// </summary>
public partial class DocumentUpload : ComponentBase
{
    [Inject]
    private IConfigurationService ConfigurationService { get; set; } = null!;

    [Inject]
    private IFileValidationService FileValidationService { get; set; } = null!;

    [Inject]
    private ILogger<DocumentUpload> Logger { get; set; } = null!;

    /// <summary>
    /// The extension type selected as a string. Used to determine required documents and fees.
    /// </summary>
    [Parameter]
    public string? ExtensionTypeString { get; set; }

    /// <summary>
    /// The waiver reason if applicable as a string. Used to determine conditional document requirements.
    /// </summary>
    [Parameter]
    public string? WaiverReasonString { get; set; }

    /// <summary>
    /// Callback fired when documents change. Passes back the updated file set.
    /// </summary>
    [Parameter]
    public EventCallback<DocumentUploadData> OnDocumentsChanged { get; set; }

    /// <summary>
    /// Callback fired when validation completes. Passes back validation result.
    /// </summary>
    [Parameter]
    public EventCallback<bool> OnValidationCompleted { get; set; }

    // File storage
    private IBrowserFile? CoverLetter { get; set; }
    private IBrowserFile? AuditorLetter { get; set; }
    private IBrowserFile? OperatorAffidavit { get; set; }
    private IBrowserFile? AdministratorLetter { get; set; }
    private IBrowserFile? LiquidatorReport { get; set; }
    private IBrowserFile? OtherDocuments { get; set; }

    // Error messages per field
    private string? CoverLetterError { get; set; }
    private string? AuditorLetterError { get; set; }
    private string? OperatorAffidavitError { get; set; }
    private string? AdministratorLetterError { get; set; }
    private string? LiquidatorReportError { get; set; }
    private string? OtherDocumentsError { get; set; }

    // Drag states for visual feedback
    private bool CoverLetterDragOver { get; set; }
    private bool AuditorLetterDragOver { get; set; }
    private bool OperatorAffidavitDragOver { get; set; }
    private bool AdministratorLetterDragOver { get; set; }
    private bool LiquidatorReportDragOver { get; set; }
    private bool OtherDocumentsDragOver { get; set; }

    // General state
    private string? ErrorMessage { get; set; }
    private string AdditionalComments { get; set; } = string.Empty;
    private DocumentRequirements? DocumentRequirements { get; set; }
    private bool ShowDocumentRequirementsInfo { get; set; }

    /// <summary>
    /// Lifecycle method called when component parameters change.
    /// Updates document requirements when extension type or waiver reason changes.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Parse extension type from string
        ExtensionType? parsedExtensionType = null;
        if (!string.IsNullOrEmpty(ExtensionTypeString))
        {
            parsedExtensionType = ConfigurationService.ParseExtensionType(ExtensionTypeString);
        }

        // Parse waiver reason from string
        WaiverReason? parsedWaiverReason = null;
        if (!string.IsNullOrEmpty(WaiverReasonString))
        {
            parsedWaiverReason = ConfigurationService.ParseWaiverReason(WaiverReasonString);
        }

        // Update document requirements based on current extension type and waiver reason
        if (parsedExtensionType.HasValue)
        {
            DocumentRequirements = ConfigurationService.GetDocumentRequirements(parsedExtensionType.Value, parsedWaiverReason);
            ShowDocumentRequirementsInfo = true;
            Logger.LogDebug("Document requirements updated for extension type {ExtensionType} and waiver {WaiverReason}",
                parsedExtensionType, parsedWaiverReason);
        }
        else
        {
            DocumentRequirements = null;
            ShowDocumentRequirementsInfo = false;
        }

        ErrorMessage = null;
        ClearAllErrors();
    }

    /// <summary>
    /// Handles drag over event for visual feedback.
    /// </summary>
    private void HandleDragOver(DragEventArgs e, string fieldName)
    {
        SetDragState(fieldName, true);
    }

    /// <summary>
    /// Handles drag leave event for visual feedback.
    /// </summary>
    private void HandleDragLeave(DragEventArgs e, string fieldName)
    {
        SetDragState(fieldName, false);
    }

    /// <summary>
    /// Handles file drop event.
    /// </summary>
    private async Task HandleDropAsync(DragEventArgs e, string fieldName)
    {
        SetDragState(fieldName, false);

        // TODO: Implement file drop handling with JS interop
        // For now, drag-drop provides visual feedback only
        // Users can click to select files
    }

    /// <summary>
    /// Handles drag over event for Cover Letter.
    /// </summary>
    private void HandleCoverLetterDragOver(DragEventArgs e) => HandleDragOver(e, "coverLetter");

    /// <summary>
    /// Handles drag leave event for Cover Letter.
    /// </summary>
    private void HandleCoverLetterDragLeave(DragEventArgs e) => HandleDragLeave(e, "coverLetter");

    /// <summary>
    /// Handles drop event for Cover Letter.
    /// </summary>
    private async Task HandleCoverLetterDrop(DragEventArgs e) => await HandleDropAsync(e, "coverLetter");

    /// <summary>
    /// Handles drag over event for Auditor Letter.
    /// </summary>
    private void HandleAuditorLetterDragOver(DragEventArgs e) => HandleDragOver(e, "auditorLetter");

    /// <summary>
    /// Handles drag leave event for Auditor Letter.
    /// </summary>
    private void HandleAuditorLetterDragLeave(DragEventArgs e) => HandleDragLeave(e, "auditorLetter");

    /// <summary>
    /// Handles drag over event for Operator Affidavit.
    /// </summary>
    private void HandleOperatorAffidavitDragOver(DragEventArgs e) => HandleDragOver(e, "operatorAffidavit");

    /// <summary>
    /// Handles drop event for Auditor Letter.
    /// </summary>
    private async Task HandleAuditorLetterDrop(DragEventArgs e) => await HandleDropAsync(e, "auditorLetter");

    /// <summary>
    /// Handles drag leave event for Operator Affidavit.
    /// </summary>
    private void HandleOperatorAffidavitDragLeave(DragEventArgs e) => HandleDragLeave(e, "operatorAffidavit");

    /// <summary>
    /// Handles drop event for Operator Affidavit.
    /// </summary>
    private async Task HandleOperatorAffidavitDrop(DragEventArgs e) => await HandleDropAsync(e, "operatorAffidavit");

    /// <summary>
    /// Handles drag over event for Liquidator Report.
    /// </summary>
    private void HandleLiquidatorReportDragOver(DragEventArgs e) => HandleDragOver(e, "liquidatorReport");

    /// <summary>
    /// Handles drag leave event for Liquidator Report.
    /// </summary>
    private void HandleLiquidatorReportDragLeave(DragEventArgs e) => HandleDragLeave(e, "liquidatorReport");

    /// <summary>
    /// Handles drop event for Liquidator Report.
    /// </summary>
    private async Task HandleLiquidatorReportDrop(DragEventArgs e) => await HandleDropAsync(e, "liquidatorReport");

    /// <summary>
    /// Handles drag over event for Administrator Letter.
    /// </summary>
    private void HandleAdministratorLetterDragOver(DragEventArgs e) => HandleDragOver(e, "administratorLetter");

    /// <summary>
    /// Handles drag leave event for Administrator Letter.
    /// </summary>
    private void HandleAdministratorLetterDragLeave(DragEventArgs e) => HandleDragLeave(e, "administratorLetter");

    /// <summary>
    /// Handles drop event for Administrator Letter.
    /// </summary>
    private async Task HandleAdministratorLetterDrop(DragEventArgs e) => await HandleDropAsync(e, "administratorLetter");

    /// <summary>
    /// Handles drag over event for Other Documents.
    /// </summary>
    private void HandleOtherDocumentsDragOver(DragEventArgs e) => HandleDragOver(e, "otherDocuments");

    /// <summary>
    /// Handles drag leave event for Other Documents.
    /// </summary>
    private void HandleOtherDocumentsDragLeave(DragEventArgs e) => HandleDragLeave(e, "otherDocuments");

    /// <summary>
    /// Handles drop event for Other Documents.
    /// </summary>
    private async Task HandleOtherDocumentsDrop(DragEventArgs e) => await HandleDropAsync(e, "otherDocuments");

    /// <summary>
    /// Sets drag state for a specific field.
    /// </summary>
    private void SetDragState(string fieldName, bool isDragOver)
    {
        switch (fieldName)
        {
            case "coverLetter":
                CoverLetterDragOver = isDragOver;
                break;
            case "auditorLetter":
                AuditorLetterDragOver = isDragOver;
                break;
            case "operatorAffidavit":
                OperatorAffidavitDragOver = isDragOver;
                break;
            case "administratorLetter":
                AdministratorLetterDragOver = isDragOver;
                break;
            case "liquidatorReport":
                LiquidatorReportDragOver = isDragOver;
                break;
            case "otherDocuments":
                OtherDocumentsDragOver = isDragOver;
                break;
        }
        StateHasChanged();
    }

    /// <summary>
    /// Handles file selection from file input. Validates file and stores or displays error.
    /// </summary>
    private async Task HandleFileChange(InputFileChangeEventArgs args, string fieldName)
    {
        try
        {
            ErrorMessage = null;
            var file = args.File;

            if (file == null)
                return;

            // Validate file using FileValidationService
            var validationResult = FileValidationService.ValidateFile(file);

            if (!validationResult.IsValid)
            {
                Logger.LogWarning("File validation failed for {FieldName}: {ErrorMessage}", fieldName, validationResult.ErrorMessage);
                SetFieldError(fieldName, validationResult.ErrorMessage);
                return;
            }

            // Store file
            SetFile(fieldName, file);
            ClearFieldError(fieldName);
            Logger.LogDebug("File accepted for {FieldName}: {FileName}", fieldName, file.Name);

            // Notify parent component of change
            await NotifyParentOfChange();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing file upload for field {FieldName}", fieldName);
            ErrorMessage = $"An error occurred while processing the file: {ex.Message}";
        }
    }

    /// <summary>
    /// Handles Cover Letter file change.
    /// </summary>
    private async Task HandleCoverLetterChange(InputFileChangeEventArgs args)
    {
        await HandleFileChange(args, "coverLetter");
    }

    /// <summary>
    /// Handles Auditor Letter file change.
    /// </summary>
    private async Task HandleAuditorLetterChange(InputFileChangeEventArgs args)
    {
        await HandleFileChange(args, "auditorLetter");
    }

    /// <summary>
    /// Handles Operator Affidavit file change.
    /// </summary>
    private async Task HandleOperatorAffidavitChange(InputFileChangeEventArgs args)
    {
        await HandleFileChange(args, "operatorAffidavit");
    }

    /// <summary>
    /// Handles Administrator Letter file change.
    /// </summary>
    private async Task HandleAdministratorLetterChange(InputFileChangeEventArgs args)
    {
        await HandleFileChange(args, "administratorLetter");
    }

    /// <summary>
    /// Handles Liquidator Report file change.
    /// </summary>
    private async Task HandleLiquidatorReportChange(InputFileChangeEventArgs args)
    {
        await HandleFileChange(args, "liquidatorReport");
    }

    /// <summary>
    /// Handles Other Documents file change.
    /// </summary>
    private async Task HandleOtherDocumentsChange(InputFileChangeEventArgs args)
    {
        await HandleFileChange(args, "otherDocuments");
    }

    /// <summary>
    /// Removes Cover Letter file.
    /// </summary>
    private async Task RemoveCoverLetter()
    {
        await RemoveFile("coverLetter");
    }

    /// <summary>
    /// Removes Auditor Letter file.
    /// </summary>
    private async Task RemoveAuditorLetter()
    {
        await RemoveFile("auditorLetter");
    }

    /// <summary>
    /// Removes Operator Affidavit file.
    /// </summary>
    private async Task RemoveOperatorAffidavit()
    {
        await RemoveFile("operatorAffidavit");
    }

    /// <summary>
    /// Removes Administrator Letter file.
    /// </summary>
    private async Task RemoveAdministratorLetter()
    {
        await RemoveFile("administratorLetter");
    }

    /// <summary>
    /// Removes Liquidator Report file.
    /// </summary>
    private async Task RemoveLiquidatorReport()
    {
        await RemoveFile("liquidatorReport");
    }

    /// <summary>
    /// Removes Other Documents file.
    /// </summary>
    private async Task RemoveOtherDocuments()
    {
        await RemoveFile("otherDocuments");
    }

    /// <summary>
    /// Removes a file from a specific field and notifies parent.
    /// </summary>
    private async Task RemoveFile(string fieldName)
    {
        SetFile(fieldName, null);
        ClearFieldError(fieldName);
        Logger.LogDebug("File removed from {FieldName}", fieldName);
        await NotifyParentOfChange();
    }

    /// <summary>
    /// Sets file for a specific field.
    /// </summary>
    private void SetFile(string fieldName, IBrowserFile? file)
    {
        switch (fieldName)
        {
            case "coverLetter":
                CoverLetter = file;
                break;
            case "auditorLetter":
                AuditorLetter = file;
                break;
            case "operatorAffidavit":
                OperatorAffidavit = file;
                break;
            case "administratorLetter":
                AdministratorLetter = file;
                break;
            case "liquidatorReport":
                LiquidatorReport = file;
                break;
            case "otherDocuments":
                OtherDocuments = file;
                break;
        }
    }

    /// <summary>
    /// Sets error message for a specific field.
    /// </summary>
    private void SetFieldError(string fieldName, string? errorMessage)
    {
        switch (fieldName)
        {
            case "coverLetter":
                CoverLetterError = errorMessage;
                break;
            case "auditorLetter":
                AuditorLetterError = errorMessage;
                break;
            case "operatorAffidavit":
                OperatorAffidavitError = errorMessage;
                break;
            case "administratorLetter":
                AdministratorLetterError = errorMessage;
                break;
            case "liquidatorReport":
                LiquidatorReportError = errorMessage;
                break;
            case "otherDocuments":
                OtherDocumentsError = errorMessage;
                break;
        }
    }

    /// <summary>
    /// Clears error for a specific field.
    /// </summary>
    private void ClearFieldError(string fieldName)
    {
        SetFieldError(fieldName, null);
    }

    /// <summary>
    /// Clears all field errors.
    /// </summary>
    private void ClearAllErrors()
    {
        CoverLetterError = null;
        AuditorLetterError = null;
        OperatorAffidavitError = null;
        AdministratorLetterError = null;
        LiquidatorReportError = null;
        OtherDocumentsError = null;
    }

    /// <summary>
    /// Validates that all required documents are present.
    /// </summary>
    public bool ValidateRequiredDocuments()
    {
        ClearAllErrors();

        if (DocumentRequirements == null)
        {
            ErrorMessage = "Document requirements not configured";
            return false;
        }

        if (CoverLetter == null)
        {
            CoverLetterError = "Cover letter is required";
        }

        if (DocumentRequirements.AuditorLetterRequired && AuditorLetter == null)
        {
            AuditorLetterError = "Auditor letter is required for this extension type";
        }

        if (DocumentRequirements.OperatorAffidavitRequired && OperatorAffidavit == null)
        {
            OperatorAffidavitError = "Operator affidavit is required for this waiver type";
        }

        if (DocumentRequirements.AdministratorLetterRequired && AdministratorLetter == null)
        {
            AdministratorLetterError = "Administrator letter is required for deregistered funds";
        }

        if (DocumentRequirements.LiquidatorReportRequired && LiquidatorReport == null)
        {
            LiquidatorReportError = "Liquidator report is required for liquidated funds";
        }

        bool allValid = CoverLetter != null &&
                        (!DocumentRequirements.AuditorLetterRequired || AuditorLetter != null) &&
                        (!DocumentRequirements.OperatorAffidavitRequired || OperatorAffidavit != null) &&
                        (!DocumentRequirements.AdministratorLetterRequired || AdministratorLetter != null) &&
                        (!DocumentRequirements.LiquidatorReportRequired || LiquidatorReport != null);

        if (!allValid)
        {
            ErrorMessage = "Please provide all required documents";
        }

        return allValid;
    }

    /// <summary>
    /// Gets the current upload data for submission.
    /// </summary>
    public DocumentUploadData GetUploadData()
    {
        return new DocumentUploadData
        {
            CoverLetter = CoverLetter,
            AuditorLetter = AuditorLetter,
            OperatorAffidavit = OperatorAffidavit,
            AdministratorLetter = AdministratorLetter,
            LiquidatorReport = LiquidatorReport,
            OtherDocuments = OtherDocuments,
            AdditionalComments = AdditionalComments
        };
    }

    /// <summary>
    /// Determines if auditor letter is required based on extension type.
    /// </summary>
    private bool IsAuditorLetterRequired() =>
        DocumentRequirements?.AuditorLetterRequired ?? false;

    /// <summary>
    /// Determines if operator affidavit is required based on waiver reason.
    /// </summary>
    private bool IsOperatorAffidavitRequired() =>
        DocumentRequirements?.OperatorAffidavitRequired ?? false;

    /// <summary>
    /// Determines if administrator letter is required for deregistered funds.
    /// </summary>
    private bool IsAdministratorLetterRequired() =>
        DocumentRequirements?.AdministratorLetterRequired ?? false;

    /// <summary>
    /// Determines if liquidator report is required for liquidated funds.
    /// </summary>
    private bool IsLiquidatorReportRequired() =>
        DocumentRequirements?.LiquidatorReportRequired ?? false;

    /// <summary>
    /// Formats bytes to human-readable format (e.g., "2.5 MB").
    /// </summary>
    private string FormatBytes(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        return $"{bytes / (1024.0 * 1024):F1} MB";
    }

    /// <summary>
    /// Gets the CSS class for drag state styling.
    /// </summary>
    private string GetDragClass(string fieldName)
    {
        var isDragOver = GetDragState(fieldName);
        var hasFile = GetFile(fieldName) != null;

        if (isDragOver)
        {
            return "border-neutral-500 bg-neutral-50";
        }
        else if (hasFile)
        {
            return "border-green-300 bg-green-50";
        }
        else
        {
            return "border-neutral-300 bg-neutral-50 hover:bg-neutral-100";
        }
    }

    /// <summary>
    /// Gets drag state for a specific field.
    /// </summary>
    private bool GetDragState(string fieldName)
    {
        return fieldName switch
        {
            "coverLetter" => CoverLetterDragOver,
            "auditorLetter" => AuditorLetterDragOver,
            "operatorAffidavit" => OperatorAffidavitDragOver,
            "administratorLetter" => AdministratorLetterDragOver,
            "liquidatorReport" => LiquidatorReportDragOver,
            "otherDocuments" => OtherDocumentsDragOver,
            _ => false
        };
    }

    /// <summary>
    /// Gets file for a specific field.
    /// </summary>
    private IBrowserFile? GetFile(string fieldName)
    {
        return fieldName switch
        {
            "coverLetter" => CoverLetter,
            "auditorLetter" => AuditorLetter,
            "operatorAffidavit" => OperatorAffidavit,
            "administratorLetter" => AdministratorLetter,
            "liquidatorReport" => LiquidatorReport,
            "otherDocuments" => OtherDocuments,
            _ => null
        };
    }

    /// <summary>
    /// Notifies parent component of document changes.
    /// </summary>
    private async Task NotifyParentOfChange()
    {
        var uploadData = GetUploadData();
        await OnDocumentsChanged.InvokeAsync(uploadData);
    }
}
