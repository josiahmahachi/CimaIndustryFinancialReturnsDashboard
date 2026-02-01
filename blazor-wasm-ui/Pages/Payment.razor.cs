using Microsoft.AspNetCore.Components;
using blazor_wasm_ui.Models;
using blazor_wasm_ui.Services;
using blazor_wasm_ui.Components;

namespace blazor_wasm_ui.Pages;

public partial class Payment : ComponentBase
{
    [Parameter]
    public string Id { get; set; } = string.Empty;

    [Inject]
    private NavigationService NavigationService { get; set; } = null!;

    [Inject]
    private IReturnsDataService ReturnsDataService { get; set; } = null!;

    private FinancialReturn? ReturnData;
    private bool Loading = true;

    // Data passed from extension request
    private string ExtensionType = string.Empty;
    private List<SubFund>? SubFunds;

    // UI state
    private List<Breadcrumb.BreadcrumbItem> BreadcrumbItems => new()
    {
        new Breadcrumb.BreadcrumbItem { Label = "Financial Returns", OnClick = HandleBreadcrumbBack },
        new Breadcrumb.BreadcrumbItem { Label = "Request Extension", OnClick = HandleBreadcrumbBack },
        new Breadcrumb.BreadcrumbItem { Label = "Payment" }
    };

    private List<ProgressIndicator.StepItem> Steps => new()
    {
        new ProgressIndicator.StepItem(1, "Request Details"),
        new ProgressIndicator.StepItem(2, "Payment"),
        new ProgressIndicator.StepItem(3, "Confirmation")
    };

    protected override async Task OnInitializedAsync()
    {
        try
        {
            ReturnData = await ReturnsDataService.GetReturnByIdAsync(Id);

            // TODO: In a real implementation, this data would be passed via query parameters,
            // navigation state, or a shared service. For now, we'll use mock data.
            // This should be replaced with proper state management in the next PBI.

            Loading = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading return: {ex.Message}");
            Loading = false;
        }
    }

    private bool ShouldSkipPayment()
    {
        if (ReturnData == null) return false;

        // Check if this is a deferral request
        if (ExtensionType == "deferral") return true;

        // For multi-fund, check if all selected sub-funds are deferrals
        if (ReturnData.FundType == FundType.MutualFund && ReturnData.FundStructure == FundStructure.MultiFund)
        {
            if (SubFunds == null || !SubFunds.Any()) return false;
            var selectedSubFunds = SubFunds.Where(sf => !string.IsNullOrEmpty(sf.ExtensionType) && sf.ExtensionType != "none");
            return selectedSubFunds.Any() && selectedSubFunds.All(sf => sf.ExtensionType == "deferral");
        }

        return false;
    }

    private void HandleBack()
    {
        NavigationService.NavigateToDashboard();
    }

    private void HandleBackToRequest()
    {
        NavigationService.NavigateToExtensionRequest(Id);
    }

    private void HandleContinueToConfirmation(PaymentData paymentData)
    {
        // TODO: In a real implementation, pass the payment data to the confirmation page
        // For now, navigate to confirmation
        NavigationService.NavigateToConfirmation();
    }

    private void HandleBreadcrumbBack()
    {
        NavigationService.NavigateToDashboard();
    }
}