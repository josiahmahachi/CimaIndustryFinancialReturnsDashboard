using blazor_wasm_ui.Models;

namespace blazor_wasm_ui.Services;

/// <summary>
/// Mock implementation of payment service for development and testing.
/// </summary>
public class PaymentService : IPaymentService
{
    public async Task<PaymentResult> ProcessPaymentAsync(PaymentData paymentData)
    {
        // Simulate payment processing delay
        await Task.Delay(2000);

        // Mock validation - in real implementation, this would call a payment gateway
        var validationResult = ValidatePaymentData(paymentData);
        if (!validationResult.IsValid)
        {
            return new PaymentResult
            {
                Success = false,
                ErrorMessage = string.Join(", ", validationResult.Errors),
                Timestamp = DateTime.UtcNow
            };
        }

        // Mock successful payment
        return new PaymentResult
        {
            Success = true,
            TransactionId = $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
            Timestamp = DateTime.UtcNow
        };
    }

    public PaymentValidationResult ValidatePaymentData(PaymentData paymentData)
    {
        var result = new PaymentValidationResult { IsValid = true };

        if (!paymentData.TermsAccepted)
        {
            result.IsValid = false;
            result.Errors.Add("Terms and conditions must be accepted");
        }

        if (paymentData.PaymentMethod == PaymentMethod.Card)
        {
            if (string.IsNullOrWhiteSpace(paymentData.CardNumber))
                result.Errors.Add("Card number is required");

            if (string.IsNullOrWhiteSpace(paymentData.ExpiryDate))
                result.Errors.Add("Expiry date is required");

            if (string.IsNullOrWhiteSpace(paymentData.Cvv))
                result.Errors.Add("CVV is required");

            if (string.IsNullOrWhiteSpace(paymentData.CardholderName))
                result.Errors.Add("Cardholder name is required");

            if (string.IsNullOrWhiteSpace(paymentData.BillingStreet) ||
                string.IsNullOrWhiteSpace(paymentData.BillingCity) ||
                string.IsNullOrWhiteSpace(paymentData.BillingPostalCode))
                result.Errors.Add("Complete billing address is required");
        }
        else if (paymentData.PaymentMethod == PaymentMethod.Escrow)
        {
            if (string.IsNullOrWhiteSpace(paymentData.EscrowProvider))
                result.Errors.Add("Escrow provider is required");

            if (string.IsNullOrWhiteSpace(paymentData.EscrowAccountNumber))
                result.Errors.Add("Escrow account number is required");
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }
}