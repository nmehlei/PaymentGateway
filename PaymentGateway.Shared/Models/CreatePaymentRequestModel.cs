using System;

namespace PaymentGateway.Shared.Models
{
    public class CreatePaymentRequestModel
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string ExternalShopperIdentifier { get; set; }

        // Credit card information,
        // NOTE: this was kept flat to limit the amount of shared models
        public string CreditCardNumber { get; set; }
        public int CreditCardExpiryMonth { get; set; }
        public int CreditCardExpiryYear { get; set; }
        public int CreditCardCcv { get; set; }
    }
}