using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentGateway.Domain.DomainServices.Randomization;
using PaymentGateway.Domain.MerchantAggregate;
using PaymentGateway.Domain.SharedValueTypes;

namespace PaymentGateway.Domain.Tests.MerchantAggregate
{
    [TestClass]
    public class MerchantTest
    {
        [TestMethod]
        public void Merchant_Is_Created_Enabled()
        {
            // Arrange
            var creditCardInformation = new CreditCardInformation("5105105105105100", new ExpiryDate(10, 2022), 667);
            var randomDS = new RandomDomainService();

            // Act
            var merchant = Merchant.RegisterNewMerchant("Test-Merchant", creditCardInformation, randomDS);

            // Assert
            Assert.IsTrue(merchant.IsEnabled);
        }

        [TestMethod]
        public void Merchant_Has_Valid_ApiKey_After_Creation()
        {
            // Arrange
            var creditCardInformation = new CreditCardInformation("5105105105105100", new ExpiryDate(10, 2022), 667);
            var randomDS = new RandomDomainService();

            // Act
            var merchant = Merchant.RegisterNewMerchant("Test-Merchant", creditCardInformation, randomDS);

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(merchant.ApiKey));
            Assert.AreEqual(22, merchant.ApiKey.Length);
        }
    }
}