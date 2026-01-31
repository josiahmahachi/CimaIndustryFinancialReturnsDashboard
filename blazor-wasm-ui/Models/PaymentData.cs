using blazor_wasm_ui.Models;

namespace blazor_wasm_ui.Models;

/// <summary>
/// Data model for payment information.
/// </summary>
public class PaymentData
{
    public PaymentMethod PaymentMethod { get; set; }
    public decimal ExtensionFee { get; set; }
    public decimal ProcessingFee { get; set; }
    public decimal BankCharges { get; set; }
    public decimal TotalAmount { get; set; }
    public bool TermsAccepted { get; set; }

    // Card payment fields
    public string? CardNumber { get; set; }
    public string? ExpiryDate { get; set; }
    public string? Cvv { get; set; }
    public string? CardholderName { get; set; }

    // Billing address
    public string? BillingStreet { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingPostalCode { get; set; }
    public string? BillingCountry { get; set; } = "Cayman Islands";

    // Wire/Bank transfer fields
    public string? TransferReference { get; set; }

    // Escrow fields
    public string? EscrowProvider { get; set; }
    public string? EscrowAccountNumber { get; set; }

    // File upload for receipts
    public string? ReceiptFileName { get; set; }
    public byte[]? ReceiptFileData { get; set; }
}