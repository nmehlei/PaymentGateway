using System;

namespace PaymentGateway.Shared.Models
{
    public class CreatePaymentResponseModel
    {
        public bool WasPaymentSuccessful { get; set; }
        public Guid PaymentId { get; set; }
    }
}