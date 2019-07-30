using Metomarket.Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metomarket.Data.EntityConfigurations
{
    internal class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        private const int CreditCardNumberMaxLength = 256;

        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.Property(contract => contract.CreditCardNumber)
                .HasMaxLength(CreditCardNumberMaxLength)
                .IsRequired();

            builder.HasOne(contract => contract.Customer)
                .WithMany(customer => customer.Contracts)
                .HasForeignKey(contract => contract.CustomerId)
                .IsRequired();

            builder.HasOne(contract => contract.Company)
                .WithMany(company => company.Contracts)
                .HasForeignKey(contract => contract.CompanyId)
                .IsRequired();
        }
    }
}