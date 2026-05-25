using AutoMapper;
using M_TAU.Application.Dtos.Transaction;
using M_TAU.Domain.Repositories;
using M_TAU.Domain.Transaction;

namespace M_TAU.Application.Services;

public sealed class FeedbackService(
    IFeedbackRepository feedbackRepository,
    IOrderRepository orderRepository,
    IMapper mapper) : IFeedbackService
{
    public async Task<FeedbackResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var feedback = await feedbackRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Feedback '{id}' not found.");
        return mapper.Map<FeedbackResponseDto>(feedback);
    }

    public async Task<IReadOnlyCollection<FeedbackResponseDto>> GetAllAsync(FeedbackFilterDto filter, CancellationToken cancellationToken = default)
    {
        var feedbacks = await feedbackRepository.ListAsync(cancellationToken);
        return mapper.Map<IReadOnlyCollection<FeedbackResponseDto>>(feedbacks);
    }

    public async Task<FeedbackResponseDto> CreateAsync(FeedbackCreateDto createDto, CancellationToken cancellationToken = default)
    {
        if (createDto.OrderId is null)
            throw new ArgumentException("OrderId is required.", nameof(createDto));

        var order = await orderRepository.GetByIdAsync(createDto.OrderId.Value, cancellationToken)
            ?? throw new KeyNotFoundException($"Order '{createDto.OrderId}' not found.");

        if (order.Status != OrderStatus.Completed)
            throw new InvalidOperationException("Feedback only allowed on completed orders.");

        var feedback = new Feedback(Guid.NewGuid(), createDto.Rating, createDto.FromUserId, createDto.OrderId);
        feedback.SetComment(createDto.Comment);
        await feedbackRepository.AddAsync(feedback, cancellationToken);
        return mapper.Map<FeedbackResponseDto>(feedback);
    }
}
