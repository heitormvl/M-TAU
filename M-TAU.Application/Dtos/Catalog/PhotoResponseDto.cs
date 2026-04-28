namespace M_TAU.Application.Dtos.Catalog;

public record PhotoResponseDto(
    Guid Id,
    string Url,
    bool IsMain
);
