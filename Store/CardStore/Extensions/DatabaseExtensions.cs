using CardStore.Data;
using CardStore.Models;
using Microsoft.EntityFrameworkCore;

namespace CardStore.Extensions;

public static class DatabaseExtensions
{
    public static async Task<IApplicationBuilder> InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            // Create database if it doesn't exist
            await context.Database.EnsureCreatedAsync();
            
            // Apply any pending migrations
            if (context.Database.GetPendingMigrations().Any())
            {
                logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync();
            }

            // Seed initial data if database is empty
            await SeedDataAsync(context, logger);
            
            logger.LogInformation("Database initialization completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }

        return app;
    }

    private static async Task SeedDataAsync(AppDbContext context, ILogger logger)
    {
        // Check if we already have data
        if (await context.Cards.AnyAsync())
        {
            logger.LogInformation("Database already contains data, skipping seeding");
            return;
        }

        logger.LogInformation("Seeding initial data...");

        // Seed sample cards
        var cards = new List<Card>
        {
            new Card
            {
                Name = "Blue-Eyes White Dragon",
                Description = "This legendary dragon is a powerful engine of destruction. Virtually invincible, very few have faced this awesome creature and lived to tell the tale.",
                Rarity = "Legendary",
                Price = 99.99m,
                Set = "Classic Set",
                Type = "Dragon",
                Attack = 3000,
                Defense = 2500,
                Level = 8,
                Attribute = "Light",
                StockQuantity = 10,
                ImageUrl = "https://example.com/blue-eyes-white-dragon.jpg"
            },
            new Card
            {
                Name = "Dark Magician",
                Description = "The ultimate wizard in terms of attack and defense.",
                Rarity = "Epic",
                Price = 79.99m,
                Set = "Classic Set",
                Type = "Spellcaster",
                Attack = 2500,
                Defense = 2100,
                Level = 7,
                Attribute = "Dark",
                StockQuantity = 15,
                ImageUrl = "https://example.com/dark-magician.jpg"
            },
            new Card
            {
                Name = "Exodia the Forbidden One",
                Description = "A forbidden monster sealed away in a statue. Few have ever been able to break the seal.",
                Rarity = "Mythic",
                Price = 199.99m,
                Set = "Forbidden Set",
                Type = "Spellcaster",
                Attack = 1000,
                Defense = 1000,
                Level = 3,
                Attribute = "Dark",
                StockQuantity = 5,
                ImageUrl = "https://example.com/exodia.jpg"
            },
            new Card
            {
                Name = "Mystical Space Typhoon",
                Description = "Destroy 1 Spell or Trap card.",
                Rarity = "Common",
                Price = 4.99m,
                Set = "Starter Deck",
                Type = "Spell",
                StockQuantity = 50,
                ImageUrl = "https://example.com/mystical-space-typhoon.jpg"
            },
            new Card
            {
                Name = "Mirror Force",
                Description = "When an opponent's monster declares an attack: Destroy all Attack Position monsters your opponent controls.",
                Rarity = "Rare",
                Price = 24.99m,
                Set = "Battle Pack",
                Type = "Trap",
                StockQuantity = 25,
                ImageUrl = "https://example.com/mirror-force.jpg"
            }
        };

        context.Cards.AddRange(cards);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {Count} sample cards", cards.Count);
    }
}