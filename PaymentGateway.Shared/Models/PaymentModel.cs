using System;

namespace PaymentGateway.Shared.Models
{
    public class PaymentModel
    {
        public Guid PaymentId { get; set; }

        public Guid MerchantId { get; set; }
        public string ExternalShopperIdentifier { get; set; }

        public PaymentStatusCode StatusCode { get; set; }

        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }

        public DateTime CreateDate { get; set; }

        public CreditCardModel CreditCard { get; set; }
    }
}