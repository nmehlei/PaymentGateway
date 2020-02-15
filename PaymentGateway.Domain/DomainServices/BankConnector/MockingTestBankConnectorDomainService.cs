using System;
using System.Threading.Tasks;
using PaymentGateway.Domain.PaymentAggregate;
using PaymentGateway.Domain.SharedValueTypes;

namespace PaymentGateway.Domain.DomainServices.BankConnector
{
    public class MockingTestBankConnectorDomainService : IBankConnectorDomainService
    {
        public MockingTestBankConnectorDomainService()
        {
            
        }

        private readonly Random _random = new Random();

        private bool DecideOrderSuccess()
        {
            const double successThreshold = 0.9;
            var randomizedOrderSuccessValue = _random.NextDouble();

            // 90% chance of success, 10% chance of failure
            return randomizedOrderSuccessValue < successThreshold;
        }

        /// <summary>
        /// Sends the payment order to the Bank to do the actual retrieval of money from the Shopper's card and payout to the Merchant.
        /// 
        /// NOTE: As this class is to be used inside the domain itself, we'll re-use the value types from the payments aggregate.
        /// This could be decoupled more with dedicated models but this would increase overall complexity and necessitate
        /// mapping between those classes thus I opted against it.
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <param name="amount"></param>
        /// <param name="sourceCreditCardInformation"></param>
        /// <param name="targetCreditCardInformation"></param>
        /// <returns></returns>
        public Task<PaymentOrderResult> TransmitPaymentOrderAsync(decimal amount, string currencyCode, CreditCardInformation sourceCreditCardInformation,
                                                                  CreditCardInformation targetCreditCardInformation)
        {
            var isSuccess = DecideOrderSuccess();

            // NOTE: we're just using a GUID as string as the payment order identifier. Could also be a timestamp,
            // randomized string or incrementing number (or any combination of those) but the application will have to
            // store it as a foreign-decided string artifact anyway as we would not have control over it (every Bank
            // could have a different algorithm and even differ inside the same Bank because of a core banking system migration)
            // thus a simple string with no direct constraint on its content should be the most future-proof storing method here.
            var paymentOrderIdentifier = Guid.NewGuid().ToString();

            // randomize if the simulated payment order succeeded. If not, simulate a rejected (or invalid) credit card.
            var resultStatus = isSuccess
                ? PaymentOrderResultStatus.Successful
                : PaymentOrderResultStatus.FailedDueToRejectedCreditCard;
            var result = new PaymentOrderResult(paymentOrderIdentifier, resultStatus);

            // NOTE: fake asynchronicity because most-likely every other non-simulated processing will benefit from asynchronous processing,
            // so the interface of this method has to be async from the Point-of-view of the caller
            return Task.FromResult(result);
        }
    }
}