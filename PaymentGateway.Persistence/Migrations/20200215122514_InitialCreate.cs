using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PaymentGateway.Persistence.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Merchants",
                columns: table => new
                {
                    MerchantId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    ApiKey = table.Column<string>(maxLength: 100, nullable: true),
                    IsEnabled = table.Column<bool>(nullable: false),
                    CreditCardInformation_CardNumber = table.Column<string>(maxLength: 100, nullable: true),
                    CreditCardInformation_ExpiryDate_Month = table.Column<int>(nullable: true),
                    CreditCardInformation_ExpiryDate_Year = table.Column<int>(nullable: true),
                    CreditCardInformation_Ccv = table.Column<int>(maxLength: 5, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.MerchantId);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(nullable: false),
                    CurrentState = table.Column<string>(maxLength: 200, nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    PaidDate = table.Column<DateTime>(nullable: true),
                    Amount_Amount = table.Column<decimal>(nullable: true),
                    Amount_CurrencyCode = table.Column<string>(maxLength: 5, nullable: true),
                    CreditCardInformation_CardNumber = table.Column<string>(maxLength: 100, nullable: true),
                    CreditCardInformation_ExpiryDate_Month = table.Column<int>(nullable: true),
                    CreditCardInformation_ExpiryDate_Year = table.Column<int>(nullable: true),
                    CreditCardInformation_Ccv = table.Column<int>(maxLength: 5, nullable: true),
                    PaymentOrderUniqueIdentifier = table.Column<string>(nullable: true),
                    MerchantId = table.Column<Guid>(nullable: false),
                    ExternalShopperIdentifier = table.Column<string>(maxLength: 200, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Merchants");

            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
