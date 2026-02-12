using FluentValidation;
using CardStore.DTOs;
using CardStore.Services;

namespace CardStore.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .Length(3, 50)
            .WithMessage("Username must be between 3 and 50 characters")
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage("Username can only contain letters, numbers, underscores, and hyphens");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Please provide a valid email address")
            .MaximumLength(100)
            .WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long")
            .MaximumLength(100)
            .WithMessage("Password cannot exceed 100 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one number");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Please confirm your password")
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match");

        RuleFor(x => x.FirstName)
            .MaximumLength(100)
            .WithMessage("First name cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^[\+]?[1-9][\d]{0,15}$")
            .WithMessage("Please provide a valid phone number")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .WithMessage("Address cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.City)
            .MaximumLength(100)
            .WithMessage("City cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.City));

        RuleFor(x => x.PostalCode)
            .MaximumLength(20)
            .WithMessage("Postal code cannot exceed 20 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.PostalCode));

        RuleFor(x => x.Country)
            .MaximumLength(100)
            .WithMessage("Country cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Country));
    }
}