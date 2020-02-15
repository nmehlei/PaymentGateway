using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentGateway.API.Helpers;
using PaymentGateway.Domain.PaymentAggregate;
using PaymentGateway.Domain.SharedValueTypes;

namespace PaymentGateway.API.Tests.Helpers
{
    [TestClass]
    public class ModelMapperTest
    {
        [TestMethod]
        public void MapAndMaskCreditCard_Returns_Valid_Model()
        {
            // Arrange
            var modelMapper = new ModelMapper();
            var creditCardInformation = new CreditCardInformation("4012888888881881", new ExpiryDate(10, 2020), 443);

            // Act
            var resultModel = modelMapper.MapAndMaskCreditCard(creditCardInformation);

            // Assert
            Assert.IsNotNull(resultModel);
            Assert.AreEqual("************1881", resultModel.CardNumber);
            Assert.AreEqual(10, resultModel.ExpiryDateMonth);
            Assert.AreEqual(2020, resultModel.ExpiryDateYear);
            Assert.AreEqual(443, resultModel.Ccv);
        }
    }
}