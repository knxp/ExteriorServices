using ExteriorServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExteriorServices.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(customer => customer.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(customer => customer.Phone)
            .HasMaxLength(30);

        builder.Property(customer => customer.Email)
            .HasMaxLength(255);

        builder.HasMany(customer => customer.Properties)
            .WithOne(property => property.Customer)
            .HasForeignKey(property => property.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}