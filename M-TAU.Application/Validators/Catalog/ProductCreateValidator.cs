using FluentValidation;
using M_TAU.Application.Dtos.Catalog;

namespace M_TAU.Application.Validators.Catalog;

public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
{
    public ProductCreateValidator()
    {
        RuleFor(x => x.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Title must not be empty or whitespace.");

        RuleFor(x => x.SellerId)
            .NotEqual(Guid.Empty)
            .WithMessage("SellerId must be a valid non-empty Guid.");

        When(x => x.TechnicalSpec is not null, () =>
        {
            RuleFor(x => x.TechnicalSpec!.WeightCapacity)
                .GreaterThanOrEqualTo(0)
                .When(x => x.TechnicalSpec!.WeightCapacity.HasValue)
                .WithMessage("WeightCapacity must be non-negative.");
        });
    }
}
