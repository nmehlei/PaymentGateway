using System;

namespace PaymentGateway.Domain.SharedValueTypes
{
    public class CreditCardInformation
    {
        public CreditCardInformation(string cardNumber, ExpiryDate expiryDate, int ccv)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                throw new ArgumentOutOfRangeException(nameof(cardNumber));
            if (expiryDate == null)
                throw new ArgumentNullException(nameof(expiryDate));
            if (ccv <= 0)
                throw new ArgumentOutOfRangeException(nameof(ccv));

            CardNumber = cardNumber;
            ExpiryDate = expiryDate;
            Ccv = ccv;
        }

        #region Persistence-internal
        /// <summary>
        /// NOTE: Unfortunately a compromise between "cleanliness" of domain model and
        /// the requirements of the underlying persistence layer
        /// </summary>
        [Obsolete("Only for Entity Framework", true)]
        private CreditCardInformation()
        {

        }
        #endregion

        public string CardNumber { get; }
        public ExpiryDate ExpiryDate { get; }
        public int Ccv { get; }
    }
}