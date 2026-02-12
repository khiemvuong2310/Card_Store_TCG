using FluentValidation;
using CardStore.DTOs;

namespace CardStore.Validators;

public class CreateCardDtoValidator : AbstractValidator<CreateCardDto>
{
    public CreateCardDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Card name is required")
            .MaximumLength(200)
            .WithMessage("Card name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Rarity)
            .NotEmpty()
            .WithMessage("Rarity is required")
            .MaximumLength(50)
            .WithMessage("Rarity cannot exceed 50 characters")
            .Must(BeValidRarity)
            .WithMessage("Rarity must be one of: Common, Uncommon, Rare, Epic, Legendary, Mythic");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Price cannot exceed $999,999.99");

        RuleFor(x => x.ImageUrl)
            .Must(BeValidUrl)
            .WithMessage("Image URL must be a valid URL")
            .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));

        RuleFor(x => x.Set)
            .NotEmpty()
            .WithMessage("Set is required")
            .MaximumLength(100)
            .WithMessage("Set name cannot exceed 100 characters");

        RuleFor(x => x.Type)
            .MaximumLength(50)
            .WithMessage("Type cannot exceed 50 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Type));

        RuleFor(x => x.Attack)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Attack cannot be negative")
            .LessThanOrEqualTo(9999)
            .WithMessage("Attack cannot exceed 9999")
            .When(x => x.Attack.HasValue);

        RuleFor(x => x.Defense)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Defense cannot be negative")
            .LessThanOrEqualTo(9999)
            .WithMessage("Defense cannot exceed 9999")
            .When(x => x.Defense.HasValue);

        RuleFor(x => x.Level)
            .InclusiveBetween(1, 12)
            .WithMessage("Level must be between 1 and 12")
            .When(x => x.Level.HasValue);

        RuleFor(x => x.Attribute)
            .MaximumLength(100)
            .WithMessage("Attribute cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Attribute));

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity cannot be negative");
    }

    private bool BeValidRarity(string rarity)
    {
        var validRarities = new[] { "Common", "Uncommon", "Rare", "Epic", "Legendary", "Mythic" };
        return validRarities.Contains(rarity, StringComparer.OrdinalIgnoreCase);
    }

    private bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;
        
        return Uri.TryCreate(url, UriKind.Absolute, out var result) && 
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}