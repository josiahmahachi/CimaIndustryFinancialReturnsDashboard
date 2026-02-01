using Microsoft.AspNetCore.Components;
using blazor_wasm_ui.Models;

namespace blazor_wasm_ui.Services
{
    public class FormStateService
    {
        // Extension Request State
        public FinancialReturn? SelectedReturn { get; set; }
        public ExtensionRequestData? ExtensionRequestData { get; set; }
        public List<SubFund>? SelectedSubFunds { get; set; }
        public PaymentData? PaymentData { get; set; }

        // Unlock Request State - handled via URL parameters, no complex state needed

        // Current Step Tracking
        public int CurrentExtensionStep { get; set; } = 1;

        // Methods for managing state
        public void StartExtensionRequest(FinancialReturn returnData)
        {
            ClearAllState();
            SelectedReturn = returnData;
            ExtensionRequestData = new ExtensionRequestData();
            CurrentExtensionStep = 1;
        }

        public void StartUnlockRequest(FinancialReturn returnData)
        {
            ClearAllState();
            SelectedReturn = returnData;
        }

        public void ClearAllState()
        {
            SelectedReturn = null;
            ExtensionRequestData = null;
            SelectedSubFunds = null;
            PaymentData = null;
            CurrentExtensionStep = 1;
        }

        public void AdvanceExtensionStep()
        {
            CurrentExtensionStep++;
        }

        public void GoBackExtensionStep()
        {
            if (CurrentExtensionStep > 1)
            {
                CurrentExtensionStep--;
            }
        }

        public bool CanAdvanceToPayment()
        {
            return !string.IsNullOrEmpty(ExtensionRequestData?.ExtensionType) &&
                   (ExtensionRequestData.ExtensionType != "waiver" ||
                    !string.IsNullOrEmpty(ExtensionRequestData.WaiverReason));
        }

        public bool CanAdvanceToConfirmation()
        {
            return PaymentData != null;
        }
    }
}