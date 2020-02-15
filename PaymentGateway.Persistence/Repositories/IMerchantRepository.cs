using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PaymentGateway.Domain.MerchantAggregate;

namespace PaymentGateway.Persistence.Repositories
{
    public interface IMerchantRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Merchant>> GetAllAsync();

        Merchant GetByMerchantId(Guid merchantId);

        Task<Merchant> GetByMerchantIdAsync(Guid merchantId);

        void Add(Merchant merchant);

        /// <summary>
        /// Persist changed data to durable storage
        /// </summary>
        Task PersistAsync();
    }
}