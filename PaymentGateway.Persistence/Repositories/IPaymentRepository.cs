using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.Domain.PaymentAggregate;

namespace PaymentGateway.Persistence.Repositories
{
    public interface IPaymentRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Payment>> GetAllAsync();

        Task<Payment> GetByPaymentIdAsync(Guid paymentId);

        void Add(Payment payment);

        /// <summary>
        /// Persist changed data to durable storage
        /// </summary>
        Task PersistAsync();
    }
}