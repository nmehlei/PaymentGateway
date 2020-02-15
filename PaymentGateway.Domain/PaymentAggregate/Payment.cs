using System;
using System.Threading.Tasks;
using PaymentGateway.Domain.DomainServices.BankConnector;
using PaymentGateway.Domain.SharedValueTypes;

namespace PaymentGateway.Domain.PaymentAggregate
{
    public class Payment
    {
        private Payment()
        {
            CreateDate = DateTime.UtcNow;
            CurrentState = PaymentState.Created;
        }

        public Guid PaymentId { get; private set; }
        public PaymentState CurrentState { get; private set; }

        public DateTime CreateDate { get; private set; }
        public DateTime? PaidDate { get; private set; }

        public PaymentAmount Amount { get; private set; }
        public CreditCardInformation CreditCardInformation { get; private set; }

        public string PaymentOrderUniqueIdentifier { get; private set; }

        public Guid MerchantId { get; private set; }
        public string ExternalShopperIdentifier { get; private set; }

        public byte[] RowVersion { get; set; }

        public static Payment Create(PaymentAmount amount, CreditCardInformation creditCardInformation, Guid merchantId, 
                                     string externalShopperIdentifier)
        {
            return new Payment
            {
                Amount = amount,
                CreditCardInformation = creditCardInformation,
                MerchantId = merchantId,
                ExternalShopperIdentifier = externalShopperIdentifier
            };
        }

        /// <summary>
        /// Deliberate Decision: Omit "Async" suffix (see link, best practice) from Aggregate methods to not blur the
        /// Ubiquitous language with technical implementation details
        /// </summary>
        /// <param name="merchantCreditCardInformation"></param>
        /// <param name="bankConnectorDS"></param>
        /// <returns></returns>
        public async Task<bool> AttemptPayment(CreditCardInformation merchantCreditCardInformation, 
                                                         IBankConnectorDomainService bankConnectorDS)
        {
            var result = await bankConnectorDS.TransmitPaymentOrderAsync(Amount.Amount, Amount.CurrencyCode, CreditCardInformation,
                merchantCreditCardInformation);

            PaymentOrderUniqueIdentifier = result.OrderUniqueIdentifier;

            switch (result.ResultStatus)
            {
                case PaymentOrderResultStatus.Successful:
                    CurrentState = PaymentState.PaymentSuccessful;
                    PaidDate = DateTime.UtcNow;
                    break;
                case PaymentOrderResultStatus.FailedDueToRejectedCreditCard:
                case PaymentOrderResultStatus.FailedDueToUnknownReason:
                    CurrentState = PaymentState.PaymentFailed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return CurrentState == PaymentState.PaymentSuccessful;
        }
    }
}