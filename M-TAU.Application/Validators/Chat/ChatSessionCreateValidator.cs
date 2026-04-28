using FluentValidation;
using M_TAU.Application.Dtos.Chat;

namespace M_TAU.Application.Validators.Chat;

public class ChatSessionCreateValidator : AbstractValidator<ChatSessionCreateDto>
{
    public ChatSessionCreateValidator()
    {
        RuleFor(x => x.BuyerId)
            .NotEqual(Guid.Empty)
            .WithMessage("BuyerId must be a valid non-empty Guid.");

        RuleFor(x => x.SellerId)
            .NotEqual(Guid.Empty)
            .WithMessage("SellerId must be a valid non-empty Guid.");

        RuleFor(x => x.ProductId)
            .NotEqual(Guid.Empty)
            .WithMessage("ProductId must be a valid non-empty Guid.");

        RuleFor(x => x)
            .Must(dto => dto.BuyerId != dto.SellerId)
            .WithMessage("Buyer and Seller must be different users.");
    }
}
