using FluentValidation;
using M_TAU.Application.Dtos.Transaction;

namespace M_TAU.Application.Validators.Transaction;

public class OrderCreateValidator : AbstractValidator<OrderCreateDto>
{
    public OrderCreateValidator()
    {
        RuleFor(x => x.BuyerId)
            .NotEqual(Guid.Empty)
            .WithMessage("BuyerId must be a valid non-empty Guid.");

        RuleFor(x => x.ProductId)
            .NotEqual(Guid.Empty)
            .WithMessage("ProductId must be a valid non-empty Guid.");

        RuleFor(x => x.TotalAmount)
            .LessThanOrEqualTo(999_999.99m)
            .WithMessage("TotalAmount must not exceed 999,999.99.");
    }
}
