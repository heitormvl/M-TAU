using FluentValidation;
using M_TAU.Application.Dtos.Transaction;

namespace M_TAU.Application.Validators.Transaction;

public class FeedbackCreateValidator : AbstractValidator<FeedbackCreateDto>
{
    public FeedbackCreateValidator()
    {
        RuleFor(x => x.FromUserId)
            .NotEqual(Guid.Empty)
            .WithMessage("FromUserId must be a valid non-empty Guid.");

        When(x => x.Comment is not null, () =>
        {
            RuleFor(x => x.Comment)
                .Must(comment => !string.IsNullOrWhiteSpace(comment))
                .WithMessage("Comment must not be empty or whitespace when provided.");
        });
    }
}
