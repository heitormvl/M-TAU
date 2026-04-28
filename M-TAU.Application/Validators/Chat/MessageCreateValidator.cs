using FluentValidation;
using M_TAU.Application.Dtos.Chat;

namespace M_TAU.Application.Validators.Chat;

public class MessageCreateValidator : AbstractValidator<MessageCreateDto>
{
    public MessageCreateValidator()
    {
        RuleFor(x => x.ChatSessionId)
            .NotEqual(Guid.Empty)
            .WithMessage("ChatSessionId must be a valid non-empty Guid.");

        RuleFor(x => x.SenderId)
            .NotEqual(Guid.Empty)
            .WithMessage("SenderId must be a valid non-empty Guid.");

        RuleFor(x => x.Content)
            .Must(content => !string.IsNullOrWhiteSpace(content))
            .WithMessage("Content must not be empty or whitespace.");
    }
}
