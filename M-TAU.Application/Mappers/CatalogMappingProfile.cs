using AutoMapper;
using M_TAU.Application.Dtos.Catalog;
using M_TAU.Domain.Catalog;

namespace M_TAU.Application.Mappers;

public class CatalogMappingProfile : Profile
{
    public CatalogMappingProfile()
    {
        CreateMap<Photo, PhotoResponseDto>();

        CreateMap<TechnicalSpec, TechnicalSpecResponseDto>();

        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos));
    }
}
