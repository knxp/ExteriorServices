using ExteriorServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExteriorServices.Infrastructure.Data.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasKey(property => property.Id);

        builder.Property(property => property.AddressLine1)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(property => property.AddressLine2)
            .HasMaxLength(200);

        builder.Property(property => property.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(property => property.State)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(property => property.PostalCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(property => property.PropertyType)
            .HasMaxLength(50);
    }
}