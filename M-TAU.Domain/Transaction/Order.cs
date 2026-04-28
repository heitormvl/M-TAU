using System.ComponentModel.DataAnnotations;
using M_TAU.Domain.Common;

namespace M_TAU.Domain.Transaction;

public class Order : EntityBase<Guid>
{
    [Required]
    public Guid BuyerId { get; private set; }

    [Required]
    public Guid ProductId { get; private set; }

    [Required]
    public DateTime OrderDate { get; private set; }

    [Required]
    public OrderStatus Status { get; private set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal TotalAmount { get; private set; }

    protected Order() { }

    public Order(Guid id, Guid buyerId, Guid productId, decimal totalAmount)
        : base(id)
    {
        BuyerId = buyerId;
        ProductId = productId;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
        SetTotalAmount(totalAmount);
    }

    public void SetTotalAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Total amount must be greater than zero.", nameof(amount));

        TotalAmount = amount;
    }

    public void UpdateStatus(OrderStatus status)
    {
        Status = status;
    }
}
