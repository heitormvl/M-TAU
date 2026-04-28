using System.ComponentModel.DataAnnotations;
using M_TAU.Domain.Common;

namespace M_TAU.Domain.Chat;

public class ChatSession : EntityBase<Guid>
{
    [Required]
    public Guid BuyerId { get; private set; }

    [Required]
    public Guid SellerId { get; private set; }

    [Required]
    public Guid ProductId { get; private set; }

    [Required]
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

    private readonly List<Message> _messages = [];

    protected ChatSession() { }

    public ChatSession(Guid id, Guid buyerId, Guid sellerId, Guid productId)
        : base(id)
    {
        BuyerId = buyerId;
        SellerId = sellerId;
        ProductId = productId;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddMessage(Message message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        _messages.Add(message);
    }
}
