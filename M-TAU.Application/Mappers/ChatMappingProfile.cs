using AutoMapper;
using M_TAU.Application.Dtos.Chat;
using M_TAU.Domain.Chat;

namespace M_TAU.Application.Mappers;

public class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        CreateMap<Message, MessageResponseDto>();

        CreateMap<ChatSession, ChatSessionResponseDto>()
            .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages));
    }
}
