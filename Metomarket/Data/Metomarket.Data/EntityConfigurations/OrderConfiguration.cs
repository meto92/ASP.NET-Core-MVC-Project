using Metomarket.Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metomarket.Data.EntityConfigurations
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(order => order.ProductId)
                .IsRequired();

            builder.HasOne(order => order.Issuer)
                .WithMany(issuer => issuer.Orders)
                .HasForeignKey(order => order.IssuerId)
                .IsRequired();
        }
    }
}