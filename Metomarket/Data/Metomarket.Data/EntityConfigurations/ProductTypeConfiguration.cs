using Metomarket.Common;
using Metomarket.Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metomarket.Data.EntityConfigurations
{
    internal class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
    {
        public void Configure(EntityTypeBuilder<ProductType> builder)
        {
            builder.Property(productType => productType.Name)
                .HasMaxLength(GlobalConstants.ProductTypeNameMaxLength)
                .IsRequired();
        }
    }
}