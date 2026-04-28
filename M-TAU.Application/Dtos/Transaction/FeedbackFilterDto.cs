namespace M_TAU.Application.Dtos.Transaction;

public record FeedbackFilterDto(
    Guid? FromUserId = null,
    Guid? OrderId = null,
    int? MinRating = null,
    int? PageNumber = null,
    int? PageSize = null
);
