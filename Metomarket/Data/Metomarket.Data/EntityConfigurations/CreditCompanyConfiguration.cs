using Metomarket.Common;
using Metomarket.Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metomarket.Data.EntityConfigurations
{
    internal class CreditCompanyConfiguration : IEntityTypeConfiguration<CreditCompany>
    {
        public void Configure(EntityTypeBuilder<CreditCompany> builder)
        {
            builder.Property(creditCompany => creditCompany.Name)
                .HasMaxLength(GlobalConstants.CreditCompanyNameMaxLength)
                .IsRequired();
        }
    }
}