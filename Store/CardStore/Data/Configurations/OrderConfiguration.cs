using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CardStore.Models;

namespace CardStore.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.UserId)
            .IsRequired();
        
        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.Property(o => o.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Pending");
        
        builder.Property(o => o.ShippingAddress)
            .HasMaxLength(200);
        
        builder.Property(o => o.ShippingCity)
            .HasMaxLength(100);
        
        builder.Property(o => o.ShippingPostalCode)
            .HasMaxLength(20);
        
        builder.Property(o => o.ShippingCountry)
            .HasMaxLength(100);
        
        builder.Property(o => o.PaymentMethod)
            .HasMaxLength(100);
        
        builder.Property(o => o.Notes)
            .HasMaxLength(500);
        
        // Create indexes for performance
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.OrderDate);
        
        // Configure relationships
        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}