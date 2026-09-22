using ExteriorServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExteriorServices.Infrastructure.Data.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(job => job.Id);

        builder.Property(job => job.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(job => job.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(job => job.Phone)
            .HasMaxLength(30);

        builder.Property(job => job.AddressLine1)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(job => job.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(job => job.PostalCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(job => job.Estimate)
            .HasColumnType("decimal(10,2)");

        builder.Property(job => job.UpFront)
            .HasColumnType("decimal(10,2)");

        builder.Property(job => job.JobTotal)
            .HasColumnType("decimal(10,2)");

        builder.HasOne(job => job.Customer)
            .WithMany()
            .HasForeignKey(job => job.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
