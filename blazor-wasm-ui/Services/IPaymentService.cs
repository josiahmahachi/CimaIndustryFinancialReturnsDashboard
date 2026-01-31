using blazor_wasm_ui.Models;

namespace blazor_wasm_ui.Services;

/// <summary>
/// Service for handling payment processing operations.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Processes a payment for an extension request.
    /// </summary>
    /// <param name="paymentData">The payment data including method and amount</param>
    /// <returns>A task representing the payment processing operation</returns>
    Task<PaymentResult> ProcessPaymentAsync(PaymentData paymentData);

    /// <summary>
    /// Validates payment data before processing.
    /// </summary>
    /// <param name="paymentData">The payment data to validate</param>
    /// <returns>Validation result with any errors</returns>
    PaymentValidationResult ValidatePaymentData(PaymentData paymentData);
}

/// <summary>
/// Result of a payment processing operation.
/// </summary>
public class PaymentResult
{
    public bool Success { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Result of payment data validation.
/// </summary>
public class PaymentValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}