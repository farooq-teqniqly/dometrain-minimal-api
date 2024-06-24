using BookLibrary.Data.Entities;
using FluentValidation;

namespace BookLibrary.Api.Validators;

public class BookValidator : AbstractValidator<Book>
{
    public BookValidator()
    {
        RuleFor(book => book.Isbn).SetValidator(new IsbnValidator());
        RuleFor(book => book.Title).NotEmpty();
        RuleFor(book => book.Author).NotEmpty();
        RuleFor(book => book.ShortDescription).NotEmpty();
        RuleFor(book => book.PageCount).GreaterThan(0);
    }
}
