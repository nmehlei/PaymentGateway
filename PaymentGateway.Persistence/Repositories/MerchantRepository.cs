using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Domain.MerchantAggregate;

namespace PaymentGateway.Persistence.Repositories
{
    public class MerchantRepository : IMerchantRepository
    {
        public MerchantRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private readonly DataContext _dataContext;

        public async Task<IEnumerable<Merchant>> GetAllAsync()
        {
            // NOTE: The ConfigureAwait is not strictly necessary as the only consumer of this method
            // is asp.net core based with the corresponding absence of a synchronization context
            // but since this class might be used by a different context in the future this is more flexible.
            return await _dataContext.Merchants.ToArrayAsync().ConfigureAwait(false);
        }

        public Merchant GetByMerchantId(Guid merchantId)
        {
            return _dataContext.Merchants.SingleOrDefault(m => m.MerchantId == merchantId);
        }

        public Task<Merchant> GetByMerchantIdAsync(Guid merchantId)
        {
            return _dataContext.Merchants.SingleOrDefaultAsync(m => m.MerchantId == merchantId);
        }

        public void Add(Merchant merchant)
        {
            _dataContext.Merchants.Add(merchant);
        }

        public Task PersistAsync()
        {
            return _dataContext.SaveChangesAsync();
        }
    }
}