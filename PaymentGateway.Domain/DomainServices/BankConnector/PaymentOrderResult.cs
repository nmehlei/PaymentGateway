using System;

namespace PaymentGateway.Domain.DomainServices.BankConnector
{
    public class PaymentOrderResult
    {
        public PaymentOrderResult(string orderUniqueIdentifier, PaymentOrderResultStatus resultStatus)
        {
            OrderUniqueIdentifier = orderUniqueIdentifier;
            ResultStatus = resultStatus;
        }

        public string OrderUniqueIdentifier { get; }
        public PaymentOrderResultStatus ResultStatus { get; }
    }
}