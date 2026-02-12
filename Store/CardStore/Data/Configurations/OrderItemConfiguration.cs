using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CardStore.Models;

namespace CardStore.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);
        
        builder.Property(oi => oi.OrderId)
            .IsRequired();
        
        builder.Property(oi => oi.CardId)
            .IsRequired();
        
        builder.Property(oi => oi.Quantity)
            .IsRequired();
        
        builder.Property(oi => oi.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        // Computed property for TotalPrice (not stored in database)
        builder.Ignore(oi => oi.TotalPrice);
        
        // Create indexes for performance
        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.CardId);
        
        // Create composite unique index to prevent duplicate order items
        builder.HasIndex(oi => new { oi.OrderId, oi.CardId }).IsUnique();
        
        // Configure relationships
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(oi => oi.Card)
            .WithMany(c => c.OrderItems)
            .HasForeignKey(oi => oi.CardId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}