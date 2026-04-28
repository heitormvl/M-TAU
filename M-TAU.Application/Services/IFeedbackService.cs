using M_TAU.Application.Dtos.Transaction;

namespace M_TAU.Application.Services;

public interface IFeedbackService
{
    Task<FeedbackResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<FeedbackResponseDto>> GetAllAsync(FeedbackFilterDto filter, CancellationToken cancellationToken = default);

    Task<FeedbackResponseDto> CreateAsync(FeedbackCreateDto createDto, CancellationToken cancellationToken = default);
}
