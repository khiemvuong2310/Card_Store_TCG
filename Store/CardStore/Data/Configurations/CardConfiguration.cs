using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CardStore.Models;

namespace CardStore.Data.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(c => c.Description)
            .HasMaxLength(1000);
        
        builder.Property(c => c.Rarity)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(c => c.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        
        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500);
        
        builder.Property(c => c.Set)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(c => c.Type)
            .HasMaxLength(50);
        
        builder.Property(c => c.Attribute)
            .HasMaxLength(100);
        
        // Create indexes for performance
        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.Set);
        builder.HasIndex(c => c.Rarity);
        builder.HasIndex(c => c.Type);
        builder.HasIndex(c => c.IsActive);
        
        // Configure relationships
        builder.HasMany(c => c.OrderItems)
            .WithOne(oi => oi.Card)
            .HasForeignKey(oi => oi.CardId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(c => c.Collections)
            .WithOne(col => col.Card)
            .HasForeignKey(col => col.CardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}