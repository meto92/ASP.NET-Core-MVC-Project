using Metomarket.Common;
using Metomarket.Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Metomarket.Data.EntityConfigurations
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(user => user.UserName)
                .HasMaxLength(GlobalConstants.UsernameMaxLength)
                .IsRequired();

            builder.Property(user => user.Address)
                .HasMaxLength(GlobalConstants.UserAddressMaxLength)
                .IsRequired();

            builder.Property(user => user.FirstName)
                .HasMaxLength(GlobalConstants.UserFirstNameMaxLength);

            builder.Property(user => user.LastName)
                .HasMaxLength(GlobalConstants.UserLastNameMaxLength);

            builder.HasOne(user => user.ShoppingCart)
                .WithOne(shoppingCart => shoppingCart.Customer)
                .HasForeignKey<ShoppingCart>(shoppingCart => shoppingCart.CustomerId)
                .IsRequired();

            builder.HasMany(user => user.Claims)
                .WithOne()
                .HasForeignKey(claim => claim.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(user => user.Logins)
                .WithOne()
                .HasForeignKey(login => login.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(user => user.Roles)
                .WithOne()
                .HasForeignKey(role => role.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}