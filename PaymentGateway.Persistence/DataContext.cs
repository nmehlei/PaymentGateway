using System;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Domain.MerchantAggregate;
using PaymentGateway.Domain.PaymentAggregate;
using PaymentGateway.Persistence.Configurations;

namespace PaymentGateway.Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        private readonly string _connectionString;

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Merchant> Merchants { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new MerchantConfiguration());

            //modelBuilder.Entity<Payment>().OwnsOne(p => p.CreditCardInformation);

            base.OnModelCreating(modelBuilder);
        }
    }
}