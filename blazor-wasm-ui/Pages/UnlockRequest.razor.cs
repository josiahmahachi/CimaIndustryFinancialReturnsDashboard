using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using blazor_wasm_ui.Models;
using blazor_wasm_ui.Services;
using blazor_wasm_ui.Components;

namespace blazor_wasm_ui.Pages;

public partial class UnlockRequest : ComponentBase
{
    [Parameter]
    public string? Id { get; set; }

    [Inject]
    public NavigationService NavigationService { get; set; } = default!;

    [Inject]
    public IReturnsDataService ReturnsDataService { get; set; } = default!;

    [Inject]
    public IConfigurationService ConfigurationService { get; set; } = default!;

    // UI State
    public List<Breadcrumb.BreadcrumbItem> BreadcrumbItems { get; set; } = new();
    public FinancialReturn? ReturnData { get; set; }

    // Form Data
    public string UnlockReason { get; set; } = "";
    public string UnlockReasonOther { get; set; } = "";
    public string? UnlockJustification { get; set; }
    public string? AdditionalComments { get; set; }
    public List<IBrowserFile>? SupportingDocuments { get; set; } = new();

    // Form State
    public string ExtensionType { get; set; } = "unlock";
    public string WaiverReason { get; set; } = "";
    public bool ShowConfirmDialog { get; set; } = false;
    public bool IsSubmitting { get; set; } = false;
    public string? ReferenceNumber { get; set; }

    // Event Callbacks for FundSummary component
    public EventCallback<string> ExtensionTypeChanged => EventCallback.Factory.Create<string>(this, HandleExtensionTypeChange);
    public EventCallback<string> WaiverReasonChanged => EventCallback.Factory.Create<string>(this, HandleWaiverReasonChange);
    public EventCallback<string> UnlockReasonChanged => EventCallback.Factory.Create<string>(this, HandleUnlockReasonChange);
    public EventCallback<string> UnlockReasonOtherChanged => EventCallback.Factory.Create<string>(this, HandleUnlockReasonOtherChange);

    // Computed Properties
    public bool CanSubmit => !string.IsNullOrEmpty(UnlockReason) &&
        (UnlockReason != "other" || !string.IsNullOrWhiteSpace(UnlockJustification));

    protected override async Task OnInitializedAsync()
    {
        await LoadReturnData();
        InitializeBreadcrumb();
    }

    private async Task LoadReturnData()
    {
        if (string.IsNullOrEmpty(Id))
        {
            NavigationService.NavigateToDashboard();
            return;
        }

        try
        {
            // In a real implementation, this would fetch from the service
            // For now, we'll create a mock return based on the ID
            ReturnData = await ReturnsDataService.GetReturnByIdAsync(Id);
        }
        catch (Exception)
        {
            // Handle error - return not found
            NavigationService.NavigateToDashboard();
        }
    }

    private void InitializeBreadcrumb()
    {
        BreadcrumbItems = new List<Breadcrumb.BreadcrumbItem>
        {
            new Breadcrumb.BreadcrumbItem { Label = "Filings & Extensions", OnClick = HandleBack },
            new Breadcrumb.BreadcrumbItem { Label = "Request Unlock" }
        };
    }

    public void HandleBack()
    {
        NavigationService.NavigateToDashboard();
    }

    public void HandleBreadcrumbClick(Breadcrumb.BreadcrumbItem item)
    {
        if (item.OnClick != null)
        {
            item.OnClick();
        }
    }

    public async Task HandleExtensionTypeChange(string value)
    {
        ExtensionType = value;
        await InvokeAsync(StateHasChanged);
    }

    public async Task HandleWaiverReasonChange(string value)
    {
        WaiverReason = value;
        await InvokeAsync(StateHasChanged);
    }

    public async Task HandleUnlockReasonChange(string value)
    {
        UnlockReason = value;
        await InvokeAsync(StateHasChanged);
    }

    public async Task HandleUnlockReasonOtherChange(string value)
    {
        UnlockReasonOther = value;
        await InvokeAsync(StateHasChanged);
    }

    public async Task HandleFileUpload(ChangeEventArgs e)
    {
        if (e.Value is not null)
        {
            // For now, we'll skip the file upload implementation
            // In a real implementation, this would handle the files
            await Task.CompletedTask;
        }
        await InvokeAsync(StateHasChanged);
    }

    public void RemoveDocument(IBrowserFile file)
    {
        SupportingDocuments?.Remove(file);
        StateHasChanged();
    }

    public void HandleShowConfirmDialog()
    {
        ShowConfirmDialog = true;
        StateHasChanged();
    }

    public async Task HandleConfirmSubmit()
    {
        ShowConfirmDialog = false;
        IsSubmitting = true;
        StateHasChanged();

        try
        {
            // Simulate API call
            await Task.Delay(2000);

            // Generate reference number
            ReferenceNumber = $"UNL-{DateTime.Now.ToString("yyyyMMddHHmmss")}";

            // Navigate to confirmation page
            NavigationService.NavigateToUnlockConfirmation(
                Id!,
                ReferenceNumber,
                GetUnlockReasonLabel(),
                UnlockJustification ?? "",
                AdditionalComments ?? "");
        }
        catch (Exception)
        {
            // Handle error
            IsSubmitting = false;
            StateHasChanged();
        }
    }

    private bool IsValidFile(IBrowserFile file)
    {
        // Check file size (10MB limit)
        if (file.Size > 10 * 1024 * 1024)
        {
            return false;
        }

        // Check file type
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        var extension = Path.GetExtension(file.Name).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }

    private string GetUnlockReasonLabel()
    {
        return UnlockReason switch
        {
            "fund-deferred" => "Fund Deferred",
            "ready-to-submit" => "Ready to Submit Outstanding filing",
            "other" => "Other (please specify)",
            _ => ""
        };
    }
}