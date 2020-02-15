using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentGateway.Domain.MerchantAggregate;

namespace PaymentGateway.Persistence.Configurations
{
    public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {
            builder.HasKey(m => m.MerchantId);

            builder.Property(m => m.Name).HasMaxLength(200);
            builder.Property(m => m.ApiKey).HasMaxLength(100);

            // NOTE: credit card information is stored in the same table for performance reasons (no Join necessary)
            // but could easily be moved to a dedicated table to conform more with normalization rules
            builder.OwnsOne(m => m.CreditCardInformation, cciBuilder =>
            {
                cciBuilder.Property(cci => cci.CardNumber).HasMaxLength(100);
                cciBuilder.OwnsOne(cci => cci.ExpiryDate, expiryDateBuilder =>
                {
                    expiryDateBuilder.Property(ed => ed.Month);
                    expiryDateBuilder.Property(ed => ed.Year);
                });
                cciBuilder.Property(cci => cci.Ccv).HasMaxLength(5);
            });

            builder.Property(m => m.RowVersion).IsRowVersion();
        }
    }
}