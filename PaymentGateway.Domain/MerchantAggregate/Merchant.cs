using System;
using PaymentGateway.Domain.DomainServices.Randomization;
using PaymentGateway.Domain.SharedValueTypes;

namespace PaymentGateway.Domain.MerchantAggregate
{
    public class Merchant
    {
        private Merchant()
        {
            IsEnabled = true;
        }

        public Guid MerchantId { get; private set; }
        public string Name { get; private set; }
        public string ApiKey { get; private set; }
        public bool IsEnabled { get; private set; }

        public CreditCardInformation CreditCardInformation { get; private set; }

        public byte[] RowVersion { get; set; }

        public static Merchant RegisterNewMerchant(string name, CreditCardInformation creditCardInformation, IRandomDomainService randomDS)
        {
            var newMerchant = new Merchant
            {
                Name = name,
                CreditCardInformation = creditCardInformation
            };

            newMerchant.GenerateApiKey(randomDS);

            return newMerchant;
        }

        private void GenerateApiKey(IRandomDomainService randomDS)
        {
            const int apiKeyLength = 22;
            ApiKey = randomDS.GenerateRandomAlphanumericString(apiKeyLength);
        }

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Disable()
        {
            IsEnabled = false;
        }
    }
}