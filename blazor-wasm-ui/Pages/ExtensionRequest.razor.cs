using blazor_wasm_ui.Models;
using Microsoft.AspNetCore.Components;
using blazor_wasm_ui.Services;

public partial class ExtensionRequest : ComponentBase
{
    [Parameter]
    public string Id { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; }

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
        NavigationManager.NavigateTo("/filings");
    }

    private void HandleContinue(ExtensionRequestData requestData)
    {
        // Store Step 1 data for potential use in later tickets
        Step1Data = requestData;

        // For now, navigate to payment page with the return ID
        // In Ticket 5, this will pass the data to the payment component
        NavigationManager.NavigateTo($"/payment/{Id}");
    }
}