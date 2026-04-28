using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.Catalog;

public record PhotoCreateDto(
    [Required]
    [Url]
    string Url,

    bool IsMain = false
);
