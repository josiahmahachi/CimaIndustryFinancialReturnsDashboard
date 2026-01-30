using Microsoft.AspNetCore.Components;
using blazor_wasm_ui.Models;
using blazor_wasm_ui.Services;

namespace blazor_wasm_ui.Components;

/// <summary>
/// SubFundsSection component manages dynamic sub-fund selection for extension requests.
/// Provides extension type selection, conditional waiver reason selection, and validation.
/// </summary>
public partial class SubFundsSection : ComponentBase
{
    [Inject]
    public IConfigurationService ConfigurationService { get; set; } = null!;

    /// <summary>
    /// List of currently selected sub-funds for the extension request.
    /// </summary>
    [Parameter]
    public List<SubFund> SubFunds { get; set; } = new();

    /// <summary>
    /// List of available sub-funds that can be added to the request.
    /// </summary>
    [Parameter]
    public List<SubFund> AvailableSubFunds { get; set; } = new();

    /// <summary>
    /// Callback fired when a sub-fund's extension type changes.
    /// </summary>
    [Parameter]
    public EventCallback<(string subFundId, string extensionType)> OnExtensionChange { get; set; }

    /// <summary>
    /// Callback fired when a sub-fund's waiver reason changes.
    /// </summary>
    [Parameter]
    public EventCallback<(string subFundId, string waiverReason)> OnWaiverReasonChange { get; set; }

    /// <summary>
    /// Callback fired when a sub-fund is added to the request.
    /// </summary>
    [Parameter]
    public EventCallback<string> OnAddSubFundCallback { get; set; }

    /// <summary>
    /// Callback fired when a sub-fund is removed from the request.
    /// </summary>
    [Parameter]
    public EventCallback<string> OnRemoveSubFundCallback { get; set; }

    /// <summary>
    /// Indicates whether any sub-fund has a waiver extension selected.
    /// Used to conditionally show the waiver reason column.
    /// </summary>
    public bool HasWaiverExtensions => SubFunds.Any(sf => sf.ExtensionType == "waiver");

    /// <summary>
    /// Calculates the new filing date based on the selected extension type.
    /// </summary>
    public string GetNewFilingDate(string extensionType)
    {
        var baseDate = new DateTime(2025, 4, 30);
        return extensionType switch
        {
            "1-month" => GetLastDayOfMonth(baseDate.AddMonths(1)).ToString("MMMM d, yyyy"),
            "2-month" => GetLastDayOfMonth(baseDate.AddMonths(2)).ToString("MMMM d, yyyy"),
            "3-month" => GetLastDayOfMonth(baseDate.AddMonths(3)).ToString("MMMM d, yyyy"),
            "waiver" => "Waiver under review",
            "deferral" => GetLastDayOfMonth(baseDate.AddMonths(5)).ToString("MMMM d, yyyy"),
            _ => "Select extension type to view new date"
        };
    }

    private DateTime GetLastDayOfMonth(DateTime date)
    {
        return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
    }

    /// <summary>
    /// Gets the number of extensions that will be used by the selected extension type.
    /// </summary>
    public int GetExtensionsToUse(string extensionType)
    {
        return extensionType switch
        {
            "1-month" => 1,
            "2-month" => 2,
            "3-month" => 3,
            "waiver" => 0,
            "deferral" => 1,
            _ => 0
        };
    }

    /// <summary>
    /// Calculates the updated number of extensions used for a sub-fund.
    /// </summary>
    public int GetUpdatedExtensionsUsed(SubFund subFund)
    {
        if (string.IsNullOrEmpty(subFund.ExtensionType)) return subFund.ExtensionsUsed;
        return subFund.ExtensionsUsed + GetExtensionsToUse(subFund.ExtensionType);
    }

    /// <summary>
    /// Handles extension type change for a sub-fund.
    /// </summary>
    private async Task OnSubFundExtensionChange(string subFundId, ChangeEventArgs e)
    {
        var extensionType = e.Value?.ToString() ?? "";
        await OnExtensionChange.InvokeAsync((subFundId, extensionType));
    }

    /// <summary>
    /// Handles waiver reason change for a sub-fund.
    /// </summary>
    private async Task OnSubFundWaiverReasonChange(string subFundId, ChangeEventArgs e)
    {
        var waiverReason = e.Value?.ToString() ?? "";
        await OnWaiverReasonChange.InvokeAsync((subFundId, waiverReason));
    }

    /// <summary>
    /// Handles adding a sub-fund to the request.
    /// </summary>
    private async Task OnAddSubFund(ChangeEventArgs e)
    {
        var subFundId = e.Value?.ToString();
        if (!string.IsNullOrEmpty(subFundId))
        {
            await OnAddSubFundCallback.InvokeAsync(subFundId);
        }
    }

    /// <summary>
    /// Handles removing a sub-fund from the request.
    /// </summary>
    private async Task OnRemoveSubFund(string subFundId)
    {
        await OnRemoveSubFundCallback.InvokeAsync(subFundId);
    }

    /// <summary>
    /// Validates the current sub-fund selections.
    /// Returns true if all selections are valid.
    /// </summary>
    public bool ValidateSubFunds()
    {
        // Check that at least one sub-fund is selected
        if (!SubFunds.Any())
        {
            return false;
        }

        // Check that each selected sub-fund has a valid extension type
        foreach (var subFund in SubFunds)
        {
            if (string.IsNullOrEmpty(subFund.ExtensionType) || subFund.ExtensionType == "none")
            {
                return false;
            }

            // If waiver is selected, waiver reason must be provided
            if (subFund.ExtensionType == "waiver" && string.IsNullOrEmpty(subFund.WaiverReason))
            {
                return false;
            }

            // Check extension limits
            var extensionsToUse = GetExtensionsToUse(subFund.ExtensionType);
            if (subFund.ExtensionsUsed + extensionsToUse > subFund.MaxExtensions)
            {
                return false;
            }

            // Check day limits
            var extensionDays = GetExtensionDays(subFund.ExtensionType);
            if (subFund.UsedExtensionDays + extensionDays > ConfigurationService.MaxExtensionDaysPerFund)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the number of extension days for a given extension type.
    /// </summary>
    private int GetExtensionDays(string extensionType)
    {
        return extensionType switch
        {
            "1-month" => 30,
            "2-month" => 60,
            "3-month" => 90,
            "waiver" => 0,
            "deferral" => 180,
            _ => 0
        };
    }

    /// <summary>
    /// Gets the list of available extension types for a sub-fund.
    /// Filters based on remaining extensions and days.
    /// </summary>
    public List<string> GetAvailableExtensionTypes(SubFund subFund)
    {
        var availableTypes = new List<string> { "none" };

        var remainingExtensions = subFund.MaxExtensions - subFund.ExtensionsUsed;
        var remainingDays = ConfigurationService.MaxExtensionDaysPerFund - subFund.UsedExtensionDays;

        if (remainingExtensions >= 1 && remainingDays >= 30)
        {
            availableTypes.Add("1-month");
        }
        if (remainingExtensions >= 2 && remainingDays >= 60)
        {
            availableTypes.Add("2-month");
        }
        if (remainingExtensions >= 3 && remainingDays >= 90)
        {
            availableTypes.Add("3-month");
        }

        availableTypes.Add("waiver");
        availableTypes.Add("deferral");

        return availableTypes;
    }

    /// <summary>
    /// Gets the display label for an extension type.
    /// </summary>
    public string GetExtensionTypeLabel(string extensionType)
    {
        return extensionType switch
        {
            "none" => "No Extension",
            "1-month" => "1 Month",
            "2-month" => "2 Months",
            "3-month" => "3 Months",
            "waiver" => "Request Waiver",
            "deferral" => "Deferral",
            _ => extensionType
        };
    }
}
