using Microsoft.AspNetCore.Components;
using blazor_wasm_ui.Models;
using blazor_wasm_ui.Services;
using blazor_wasm_ui.Components;

namespace blazor_wasm_ui.Pages;

public partial class Confirmation : ComponentBase
{
    [Parameter]
    public string Id { get; set; } = string.Empty;

    [Inject]
    public IConfigurationService ConfigurationService { get; set; } = null!;

    [Inject]
    private IReturnsDataService ReturnsDataService { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    private FinancialReturn? ReturnData;
    private string ExtensionType = string.Empty;
    private List<SubFund>? SubFunds;
    private PaymentData? PaymentData;
    private bool Loading = true;

    // UI state
    private List<Breadcrumb.BreadcrumbItem> BreadcrumbItems => new()
    {
        new Breadcrumb.BreadcrumbItem { Label = "Financial Returns", OnClick = HandleBreadcrumbBack },
        new Breadcrumb.BreadcrumbItem { Label = "Request Extension", OnClick = HandleBreadcrumbBack },
        new Breadcrumb.BreadcrumbItem { Label = "Payment", OnClick = HandleBreadcrumbBack },
        new Breadcrumb.BreadcrumbItem { Label = "Confirmation" }
    };

    private List<ProgressIndicator.StepItem> Steps => new()
    {
        new ProgressIndicator.StepItem(1, "Request Details"),
        new ProgressIndicator.StepItem(2, "Payment"),
        new ProgressIndicator.StepItem(3, "Confirmation")
    };

    private string ReferenceNumber => $"EXT-{DateTime.Now:yyyyMMddHHmmss}";
    private bool IsMultiFund => ReturnData?.FundType == FundType.MutualFund && ReturnData?.FundStructure == FundStructure.MultiFund;
    private bool IsDeferralRequest => ExtensionType == "deferral";

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(Id))
            {
                ReturnData = await ReturnsDataService.GetReturnByIdAsync(Id);
            }

            // TODO: In a real implementation, extension and payment data would be passed
            // via navigation state or a shared service. For now, we'll use mock data.
            // This should be replaced with proper state management in the next PBI.

            Loading = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading confirmation data: {ex.Message}");
            Loading = false;
        }
    }

    private string GetNewFilingDate(string extensionType)
    {
        var baseDate = new DateTime(2025, 4, 30);
        return extensionType switch
        {
            "1-month" => baseDate.AddMonths(1).ToString("MMMM d, yyyy"),
            "2-month" => baseDate.AddMonths(2).ToString("MMMM d, yyyy"),
            "3-month" => baseDate.AddMonths(3).ToString("MMMM d, yyyy"),
            "deferral" => baseDate.AddMonths(6).ToString("MMMM d, yyyy"),
            _ => ReturnData?.DueDate ?? "Unknown"
        };
    }

    private string GetExtensionTypeDisplay(string extensionType)
    {
        return extensionType switch
        {
            "1-month" => "1 Month Extension (30 days)",
            "2-month" => "2 Month Extension (60 days)",
            "3-month" => "3 Month Extension (90 days)",
            "waiver" => "Request Waiver",
            "deferral" => "Deferral (6 months)",
            _ => "No Extension"
        };
    }

    private string GetPaymentMethodDisplay()
    {
        if (PaymentData == null) return "N/A";

        var method = PaymentData.PaymentMethod switch
        {
            PaymentMethod.Card => "Card Payment",
            PaymentMethod.WireTransfer => "Wire Transfer",
            PaymentMethod.BankTransfer => "Bank Transfer",
            PaymentMethod.Escrow => "Escrow",
            _ => "Unknown"
        };

        if (PaymentData.PaymentMethod == PaymentMethod.Card && !string.IsNullOrEmpty(PaymentData.CardNumber))
        {
            var lastFour = PaymentData.CardNumber.Length >= 4
                ? PaymentData.CardNumber[^4..]
                : PaymentData.CardNumber;
            method += $" (**** **** **** {lastFour})";
        }

        return method;
    }

    private string GetPaymentStatus()
    {
        if (PaymentData == null) return "N/A";

        return PaymentData.PaymentMethod == PaymentMethod.Escrow ? "Completed" : "Pending Review";
    }

    private string GetTransactionId()
    {
        if (PaymentData == null) return "N/A";

        return PaymentData.PaymentMethod switch
        {
            PaymentMethod.Card => $"CARD-{DateTime.Now:yyyyMMddHHmmss}",
            PaymentMethod.WireTransfer => PaymentData.TransferReference ?? $"WIRE-{DateTime.Now:yyyyMMddHHmmss}",
            PaymentMethod.BankTransfer => PaymentData.TransferReference ?? $"BANK-{DateTime.Now:yyyyMMddHHmmss}",
            PaymentMethod.Escrow => PaymentData.EscrowAccountNumber ?? $"ESCR-{DateTime.Now:yyyyMMddHHmmss}",
            _ => $"TXN-{DateTime.Now:yyyyMMddHHmmss}"
        };
    }

    private string FormatCurrency(decimal amount)
    {
        return $"{amount:N2} CI$";
    }

    private string FormatTimestamp(DateTime timestamp)
    {
        return timestamp.ToString("MMMM d, yyyy 'at' h:mm tt");
    }

    private async Task HandleDownloadReceipt()
    {
        // In a real implementation, this would generate and download a PDF
        // For now, just show an alert
        await Task.CompletedTask;
    }

    private void HandleBackToDashboard()
    {
        NavigationManager.NavigateTo("/filings");
    }

    private void HandleBreadcrumbBack()
    {
        NavigationManager.NavigateTo("/filings");
    }

    private string GetPaymentStatusClass()
    {
        if (PaymentData == null) return "bg-gray-100 text-gray-800";

        return PaymentData.PaymentMethod == PaymentMethod.Escrow
            ? "bg-green-100 text-green-800"
            : "bg-orange-100 text-orange-800";
    }
}