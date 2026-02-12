using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CardStore.Models;

namespace CardStore.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.PasswordHash)
            .IsRequired();
        
        builder.Property(u => u.FirstName)
            .HasMaxLength(100);
        
        builder.Property(u => u.LastName)
            .HasMaxLength(100);
        
        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(15);
        
        builder.Property(u => u.Address)
            .HasMaxLength(500);
        
        builder.Property(u => u.City)
            .HasMaxLength(100);
        
        builder.Property(u => u.PostalCode)
            .HasMaxLength(20);
        
        builder.Property(u => u.Country)
            .HasMaxLength(100);
        
        // Create unique indexes
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        
        // Configure relationships
        builder.HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(u => u.Collections)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}