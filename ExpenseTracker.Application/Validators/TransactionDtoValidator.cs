using FluentValidation;
using ExpenseTracker.Application.DTOs;

public class TransactionDtoValidator : AbstractValidator<TransactionDto>
{
    public TransactionDtoValidator()
    {
        RuleFor(x => x.Amount)
            .NotNull()
            .WithMessage("Amount is required.")
            .NotEqual(0).WithMessage("Amount cannot be zero.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description can't be longer than 500 characters.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be a positive number.");
    }
}