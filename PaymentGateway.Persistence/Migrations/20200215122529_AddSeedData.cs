using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentGateway.Persistence.Migrations
{
    public partial class AddSeedData : Migration
    {
        private static readonly Guid _exampleMerchantId1 = Guid.Parse("02177E41-013E-45F6-86C9-F36BDECB5D05");
        private static readonly Guid _exampleMerchantId2 = Guid.Parse("E518BCD4-769E-4E08-BCF5-A7415E9FD89E");
        private static readonly Guid _exampleMerchantId3 = Guid.Parse("EA89DB31-6006-4157-AFF4-D71E7489A806");

        private static readonly Guid _examplePaymentId1 = Guid.Parse("CF8A1637-5A06-4179-8BB2-7E6056A344E5");

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add merchant seed data

            migrationBuilder.InsertData(table: "Merchants",
                columns: new[]
                {
                    "MerchantId", "Name", "ApiKey", "IsEnabled", "CreditCardInformation_CardNumber",
                    "CreditCardInformation_Ccv", "CreditCardInformation_ExpiryDate_Month", "CreditCardInformation_ExpiryDate_Year"
                },
                values: new object[]
                {
                    _exampleMerchantId1, "Contoso Inc.", "oWq04oaqA64HZzIxXzWw", true, "378282246310005", 334, 10, 2022
                });

            migrationBuilder.InsertData(table: "Merchants",
                columns: new[] {
                    "MerchantId", "Name", "ApiKey", "IsEnabled", "CreditCardInformation_CardNumber",
                    "CreditCardInformation_Ccv", "CreditCardInformation_ExpiryDate_Month", "CreditCardInformation_ExpiryDate_Year"
                },
                values: new object[]
                {
                    _exampleMerchantId2, "Northwind Corp.", "Cxs557f7OMPM9o7iHjcH", true, "378282246310005", 468, 4, 2023
                });

            migrationBuilder.InsertData(table: "Merchants",
                columns: new[] {
                    "MerchantId", "Name", "ApiKey", "IsEnabled", "CreditCardInformation_CardNumber",
                    "CreditCardInformation_Ccv", "CreditCardInformation_ExpiryDate_Month", "CreditCardInformation_ExpiryDate_Year"
                },
                values: new object[]
                {
                    _exampleMerchantId3, "TestCompany", "6UEP1Q71gA6S2XO9S7dP", true, "378282246310005", 265, 7, 2020
                });

            // Add payment seed data

            migrationBuilder.InsertData(table: "Payments",
                columns: new[]
                {
                    "PaymentId", "CurrentState", "CreateDate", "PaidDate", "Amount_Amount", "Amount_CurrencyCode",
                    "CreditCardInformation_CardNumber", "CreditCardInformation_ExpiryDate_Month", "CreditCardInformation_ExpiryDate_Year",
                    "CreditCardInformation_Ccv", "MerchantId", "ExternalShopperIdentifier"
                },
                values: new object[]
                {
                    _examplePaymentId1, "PaymentSuccessful", DateTime.UtcNow.AddHours(-3), DateTime.UtcNow.AddHours(-3).AddSeconds(10),
                    500, "EUR", "4012888888881881", 2, 2023, 889, _exampleMerchantId3, "XX-2323534535-N"
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Merchants", "MerchantId", _exampleMerchantId1);
            migrationBuilder.DeleteData(table: "Merchants", "MerchantId", _exampleMerchantId2);
            migrationBuilder.DeleteData(table: "Merchants", "MerchantId", _exampleMerchantId3);

            migrationBuilder.DeleteData(table: "Payments", keyColumn: "PaymentId", keyValue: _examplePaymentId1);
        }
    }
}