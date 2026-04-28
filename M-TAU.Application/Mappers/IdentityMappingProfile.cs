using AutoMapper;
using M_TAU.Application.Dtos.Identity;
using M_TAU.Domain.Identity;

namespace M_TAU.Application.Mappers;

public class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        CreateMap<User, UserResponseDto>();
    }
}
