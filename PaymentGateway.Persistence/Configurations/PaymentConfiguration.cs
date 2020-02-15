using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PaymentGateway.Domain.PaymentAggregate;

namespace PaymentGateway.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.PaymentId);

            builder.Property(p => p.CurrentState)
                   .HasMaxLength(200)
                   .HasConversion(new EnumToStringConverter<PaymentState>());

            builder.Property(p => p.ExternalShopperIdentifier)
                   .HasMaxLength(200);

            // NOTE: credit card information is stored in the same table for performance reasons (no Join necessary)
            // but could easily be moved to a dedicated table to conform more with normalization rules
            builder.OwnsOne(p => p.Amount, paBuilder =>
            {
                paBuilder.Property(pa => pa.Amount);
                paBuilder.Property(pa => pa.CurrencyCode).HasMaxLength(5);
            });

            builder.OwnsOne(p => p.CreditCardInformation, cciBuilder =>
            {
                cciBuilder.Property(cci => cci.CardNumber).HasMaxLength(100);
                cciBuilder.OwnsOne(cci => cci.ExpiryDate, expiryDateBuilder =>
                {
                    expiryDateBuilder.Property(ed => ed.Month);
                    expiryDateBuilder.Property(ed => ed.Year);
                });
                cciBuilder.Property(cci => cci.Ccv).HasMaxLength(5);
            });

            builder.Property(p => p.RowVersion).IsRowVersion();
        }
    }
}