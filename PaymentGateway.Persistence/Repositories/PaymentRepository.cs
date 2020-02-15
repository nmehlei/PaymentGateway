using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Domain.PaymentAggregate;

namespace PaymentGateway.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        public PaymentRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private readonly DataContext _dataContext;

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            // NOTE: The ConfigureAwait is not strictly necessary as the only consumer of this method
            // is asp.net core based with the corresponding absence of a synchronization context
            // but since this class might be used by a different context in the future this is more flexible.
            return await _dataContext.Payments.ToListAsync().ConfigureAwait(false);
        }

        public Task<Payment> GetByPaymentIdAsync(Guid paymentId)
        {
            return _dataContext.Payments.SingleOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public void Add(Payment payment)
        {
            _dataContext.Payments.Add(payment);
        }

        public Task PersistAsync()
        {
            return _dataContext.SaveChangesAsync();
        }
    }
}