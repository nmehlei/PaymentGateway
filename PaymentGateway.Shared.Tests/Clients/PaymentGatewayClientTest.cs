using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentGateway.Domain.MerchantAggregate;
using PaymentGateway.Shared.Clients;

namespace PaymentGateway.Shared.Tests.Clients
{
    [TestClass]
    public class PaymentGatewayClientTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PaymentGatewayClient_Instantiation_Fails_With_Missing_MerchantId()
        {
            // Arrange
            var merchantId = Guid.Empty;
            var apiKey = "xxxx";

            // Act
            var client = new PaymentGatewayClient(merchantId, apiKey);

            // Assert
        }
    }
}
