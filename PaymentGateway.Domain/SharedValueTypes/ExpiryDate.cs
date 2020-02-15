using System;

namespace PaymentGateway.Domain.SharedValueTypes
{
    public class ExpiryDate
    {
        public ExpiryDate(int month, int year)
        {
            Month = month;
            Year = year;
        }

        #region Persistence-internal
        /// <summary>
        /// NOTE: Unfortunately a compromise between "cleanliness" of domain model and
        /// the requirements of the underlying persistence layer
        /// </summary>
        [Obsolete("Only for Entity Framework", true)]
        private ExpiryDate()
        {

        }
        #endregion

        public int Month { get; }
        public int Year { get; }
    }
}