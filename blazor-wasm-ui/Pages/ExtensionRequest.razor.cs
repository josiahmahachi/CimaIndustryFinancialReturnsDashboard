using blazor_wasm_ui.Models;
using Microsoft.AspNetCore.Components;
using blazor_wasm_ui.Services;

public partial class ExtensionRequest : ComponentBase
{
    [Parameter]
    public string Id { get; set; }

    [Inject]
    private NavigationService NavigationService { get; set; }

    [Inject]
    private FormStateService FormStateService { get; set; }

    [Inject]
    private IReturnsDataService ReturnsDataService { get; set; }

    private FinancialReturn? ReturnData;
    private bool Loading = true;

    // Step 1 data storage
    private ExtensionRequestData? Step1Data;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            ReturnData = await ReturnsDataService.GetReturnByIdAsync(Id);
            Loading = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading return: {ex.Message}");
            Loading = false;
        }
    }

    private void HandleBack()
    {
        NavigationService.NavigateToFilings();
    }

    private void HandleContinue(ExtensionRequestData requestData)
    {
        // Store Step 1 data in FormStateService
        FormStateService.ExtensionRequestData = requestData;
        FormStateService.AdvanceExtensionStep();

        // Navigate to payment page
        NavigationService.NavigateToPayment();
    }
}