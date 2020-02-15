using System;
using System.Threading.Tasks;
using PaymentGateway.Domain.SharedValueTypes;

namespace PaymentGateway.Domain.DomainServices.BankConnector
{
    public interface IBankConnectorDomainService
    {
        /// <summary>
        /// Sends the payment order to the Bank to do the actual retrieval of money from the Shopper's card and payout to the Merchant.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="currencyCode"></param>
        /// <param name="sourceCreditCardInformation"></param>
        /// <param name="targetCreditCardInformation"></param>
        /// <returns></returns>
        Task<PaymentOrderResult> TransmitPaymentOrderAsync(decimal amount, string currencyCode, CreditCardInformation sourceCreditCardInformation,
                                                           CreditCardInformation targetCreditCardInformation);
    }
}