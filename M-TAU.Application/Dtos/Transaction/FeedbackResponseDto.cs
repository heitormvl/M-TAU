namespace M_TAU.Application.Dtos.Transaction;

public record FeedbackResponseDto(
    Guid Id,
    int Rating,
    Guid FromUserId,
    string? Comment = null,
    Guid? OrderId = null
);
