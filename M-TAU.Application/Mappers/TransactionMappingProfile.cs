using AutoMapper;
using M_TAU.Application.Dtos.Transaction;
using M_TAU.Domain.Transaction;

namespace M_TAU.Application.Mappers;

public class TransactionMappingProfile : Profile
{
    public TransactionMappingProfile()
    {
        CreateMap<Order, OrderResponseDto>();

        CreateMap<Feedback, FeedbackResponseDto>();
    }
}
