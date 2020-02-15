using System;

namespace PaymentGateway.Domain.PaymentAggregate
{
    public class PaymentAmount
    {
        public PaymentAmount(decimal amount, string currencyCode)
        {
            Amount = amount;
            CurrencyCode = currencyCode;
        }

        #region Persistence-internal
        /// <summary>
        /// NOTE: Unfortunately a compromise between "cleanliness" of domain model and
        /// the requirements of the underlying persistence layer
        /// </summary>
        [Obsolete("Only for Entity Framework", true)]
        private PaymentAmount()
        {

        }
        #endregion

        public decimal Amount { get; }
        public string CurrencyCode { get; }
    }
}