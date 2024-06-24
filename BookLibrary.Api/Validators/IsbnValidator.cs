using FluentValidation;

namespace BookLibrary.Api.Validators;

public class IsbnValidator : AbstractValidator<string>
{
    public IsbnValidator()
    {
        RuleFor(isbn => isbn)
            .Matches(@"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$")
            .WithMessage("Value is not a valid ISBN-13.");
    }
}
