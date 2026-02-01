using Microsoft.AspNetCore.Components;
using blazor_wasm_ui.Models;

namespace blazor_wasm_ui.Services
{
    public class NavigationService
    {
        private readonly NavigationManager _navigationManager;

        public NavigationService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public void NavigateToDashboard() => _navigationManager.NavigateTo("/filings");
        public void NavigateToFilings() => _navigationManager.NavigateTo("/filings");
        public void NavigateToReports() => _navigationManager.NavigateTo("/reports");
        public void NavigateToExtensionRequest(string returnId) => _navigationManager.NavigateTo($"/extension-request/{returnId}");
        public void NavigateToPayment() => _navigationManager.NavigateTo("/payment");
        public void NavigateToPayment(string returnId) => _navigationManager.NavigateTo($"/payment/{returnId}");
        public void NavigateToConfirmation() => _navigationManager.NavigateTo("/confirmation");
        public void NavigateToUnlockRequest(string returnId) => _navigationManager.NavigateTo($"/unlock-request/{returnId}");
        public void NavigateToUnlockConfirmation(string returnId, string referenceNumber, string reason, string justification, string comments) =>
            _navigationManager.NavigateTo($"/unlock-confirmation/{returnId}?ref={referenceNumber}&reason={reason}&justification={Uri.EscapeDataString(justification)}&comments={Uri.EscapeDataString(comments)}");
        public void NavigateToExtensionView(string returnId) => _navigationManager.NavigateTo($"/extension-view/{returnId}");
        public void NavigateToUIDemo() => _navigationManager.NavigateTo("/ui-demo");
    }
}