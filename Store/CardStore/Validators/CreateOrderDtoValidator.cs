using FluentValidation;
using CardStore.DTOs;

namespace CardStore.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.OrderItems)
            .NotEmpty()
            .WithMessage("Order must contain at least one item")
            .Must(x => x.Count <= 50)
            .WithMessage("Order cannot contain more than 50 different items");

        RuleForEach(x => x.OrderItems)
            .SetValidator(new CreateOrderItemDtoValidator());

        RuleFor(x => x.ShippingAddress)
            .MaximumLength(200)
            .WithMessage("Shipping address cannot exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.ShippingAddress));

        RuleFor(x => x.ShippingCity)
            .MaximumLength(100)
            .WithMessage("Shipping city cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.ShippingCity));

        RuleFor(x => x.ShippingPostalCode)
            .MaximumLength(20)
            .WithMessage("Shipping postal code cannot exceed 20 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.ShippingPostalCode));

        RuleFor(x => x.ShippingCountry)
            .MaximumLength(100)
            .WithMessage("Shipping country cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.ShippingCountry));

        RuleFor(x => x.PaymentMethod)
            .MaximumLength(100)
            .WithMessage("Payment method cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.PaymentMethod));

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.CardId)
            .GreaterThan(0)
            .WithMessage("Card ID must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Quantity cannot exceed 100 for a single item");
    }
}