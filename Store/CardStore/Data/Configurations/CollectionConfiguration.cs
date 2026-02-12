using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CardStore.Models;

namespace CardStore.Data.Configurations;

public class CollectionConfiguration : IEntityTypeConfiguration<Collection>
{
    public void Configure(EntityTypeBuilder<Collection> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.UserId)
            .IsRequired();
        
        builder.Property(c => c.CardId)
            .IsRequired();
        
        builder.Property(c => c.Quantity)
            .IsRequired();
        
        builder.Property(c => c.Notes)
            .HasMaxLength(500);
        
        // Create indexes for performance
        builder.HasIndex(c => c.UserId);
        builder.HasIndex(c => c.CardId);
        builder.HasIndex(c => c.AcquiredDate);
        
        // Create composite unique index to prevent duplicate collections
        builder.HasIndex(c => new { c.UserId, c.CardId }).IsUnique();
        
        // Configure relationships
        builder.HasOne(c => c.User)
            .WithMany(u => u.Collections)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(c => c.Card)
            .WithMany(card => card.Collections)
            .HasForeignKey(c => c.CardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}