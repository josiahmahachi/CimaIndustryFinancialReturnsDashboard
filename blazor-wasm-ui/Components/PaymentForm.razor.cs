using Microsoft.AspNetCore.Components;
using blazor_wasm_ui.Models;
using blazor_wasm_ui.Services;

namespace blazor_wasm_ui.Components;

public partial class PaymentForm : ComponentBase
{
    [Inject]
    public IConfigurationService ConfigurationService { get; set; } = null!;

    [Inject]
    public IPaymentService PaymentService { get; set; } = null!;

    [Parameter]
    public FinancialReturn ReturnData { get; set; } = null!;

    [Parameter]
    public string ExtensionType { get; set; } = string.Empty;

    [Parameter]
    public List<SubFund>? SubFunds { get; set; }

    [Parameter]
    public EventCallback OnBack { get; set; }

    [Parameter]
    public EventCallback<PaymentData> OnContinue { get; set; }

    private PaymentData paymentData = new();
    private bool isProcessing = false;
    private string? errorMessage;

    protected override void OnInitialized()
    {
        paymentData = new PaymentData
        {
            PaymentMethod = PaymentMethod.Card,
            BillingCountry = "Cayman Islands",
            TermsAccepted = false
        };

        // Calculate fees
        CalculateFees();
    }

    private void CalculateFees()
    {
        var extensionFee = CalculateExtensionFee();
        var processingFee = extensionFee > 0 ? ConfigurationService.ProcessingFee : 0;
        var bankCharges = paymentData.PaymentMethod == PaymentMethod.WireTransfer ? ConfigurationService.WireTransferBankCharges : 0;

        paymentData.ExtensionFee = extensionFee;
        paymentData.ProcessingFee = processingFee;
        paymentData.BankCharges = bankCharges;
        paymentData.TotalAmount = extensionFee + processingFee + bankCharges;
    }

    private decimal CalculateExtensionFee()
    {
        var isMultiFund = ReturnData.FundType == FundType.MutualFund && ReturnData.FundStructure == FundStructure.MultiFund;

        if (!isMultiFund)
        {
            // Single fund calculation
            return ExtensionType switch
            {
                "1-month" => ConfigurationService.BaseExtensionFeePerMonth,
                "2-month" => ConfigurationService.BaseExtensionFeePerMonth * 2,
                "3-month" => ConfigurationService.BaseExtensionFeePerMonth * 3,
                "waiver" => 0,
                "deferral" => 0,
                _ => 0
            };
        }
        else
        {
            // Multi-fund calculation
            if (SubFunds == null || !SubFunds.Any())
                return 0;

            decimal totalFee = 0;
            foreach (var subFund in SubFunds)
            {
                if (!string.IsNullOrEmpty(subFund.ExtensionType) && subFund.ExtensionType != "none")
                {
                    var subFundFee = subFund.ExtensionType switch
                    {
                        "1-month" => ConfigurationService.BaseExtensionFeePerMonth,
                        "2-month" => ConfigurationService.BaseExtensionFeePerMonth * 2,
                        "3-month" => ConfigurationService.BaseExtensionFeePerMonth * 3,
                        "waiver" => 0,
                        "deferral" => 0,
                        _ => 0
                    };
                    totalFee += subFundFee;
                }
            }
            return totalFee;
        }
    }

    private void OnPaymentMethodChanged(PaymentMethod method)
    {
        paymentData.PaymentMethod = method;
        CalculateFees();
        StateHasChanged();
    }

    private async Task HandleSubmit()
    {
        if (isProcessing) return;

        errorMessage = null;
        isProcessing = true;

        try
        {
            var validationResult = PaymentService.ValidatePaymentData(paymentData);
            if (!validationResult.IsValid)
            {
                errorMessage = string.Join(", ", validationResult.Errors);
                return;
            }

            var result = await PaymentService.ProcessPaymentAsync(paymentData);
            if (result.Success)
            {
                await OnContinue.InvokeAsync(paymentData);
            }
            else
            {
                errorMessage = result.ErrorMessage ?? "Payment processing failed";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            isProcessing = false;
            StateHasChanged();
        }
    }

    private string FormatCurrency(decimal amount)
    {
        return $"{amount:N2} CI$";
    }

    private string GetPaymentMethodLabel(PaymentMethod method)
    {
        return method switch
        {
            PaymentMethod.Card => "Card Payment",
            PaymentMethod.WireTransfer => "Wire Transfer",
            PaymentMethod.BankTransfer => "Bank Transfer",
            PaymentMethod.Escrow => "Escrow",
            _ => method.ToString()
        };
    }

    private string FormattedCardNumber
    {
        get => paymentData.CardNumber ?? "";
        set
        {
            var digits = new string(value.Where(char.IsDigit).ToArray());
            var formatted = string.Join("-", digits.Select((c, i) => new { c, i })
                .GroupBy(x => x.i / 4)
                .Select(g => new string(g.Select(x => x.c).ToArray())));
            paymentData.CardNumber = formatted.Length > 19 ? formatted.Substring(0, 19) : formatted;
        }
    }

    private string FormattedExpiryDate
    {
        get => paymentData.ExpiryDate ?? "";
        set
        {
            var digits = new string(value.Where(char.IsDigit).ToArray());
            if (digits.Length >= 2)
            {
                paymentData.ExpiryDate = $"{digits.Substring(0, 2)}/{digits.Substring(2, Math.Min(2, digits.Length - 2))}";
            }
            else
            {
                paymentData.ExpiryDate = digits;
            }
        }
    }
}