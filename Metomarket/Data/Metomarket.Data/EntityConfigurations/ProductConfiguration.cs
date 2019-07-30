using Metomarket.Common;
using Metomarket.Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metomarket.Data.EntityConfigurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(product => product.Name)
                .HasMaxLength(GlobalConstants.ProductNameMaxLength)
                .IsRequired();

            builder.Property(product => product.ImageUrl)
                .HasMaxLength(GlobalConstants.ProductImageUrlMaxLength)
                .IsRequired();

            builder.HasOne(product => product.Type)
                .WithMany(type => type.Products)
                .HasForeignKey(product => product.TypeId)
                .IsRequired();
        }
    }
}