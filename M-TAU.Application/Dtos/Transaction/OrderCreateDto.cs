using System.ComponentModel.DataAnnotations;

namespace M_TAU.Application.Dtos.Transaction;

public record OrderCreateDto(
    [Required]
    Guid BuyerId,

    [Required]
    Guid ProductId,

    [Required]
    [Range(0.01, double.MaxValue)]
    decimal TotalAmount
);
