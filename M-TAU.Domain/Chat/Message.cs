using System.ComponentModel.DataAnnotations;
using M_TAU.Domain.Common;

namespace M_TAU.Domain.Chat;

public class Message : EntityBase<Guid>
{
    [Required]
    public Guid ChatSessionId { get; private set; }

    [Required]
    public Guid SenderId { get; private set; }

    [Required]
    [StringLength(5000)]
    public string Content { get; private set; } = string.Empty;

    [Required]
    public DateTime SentAt { get; private set; }

    [Required]
    public MessageStatus Status { get; private set; }

    protected Message() { }

    public Message(Guid id, Guid chatSessionId, Guid senderId, string content)
        : base(id)
    {
        ChatSessionId = chatSessionId;
        SenderId = senderId;
        SetContent(content);
        SentAt = DateTime.UtcNow;
        Status = MessageStatus.Sent;
    }

    public void SetContent(string content)
    {
        Content = string.IsNullOrWhiteSpace(content)
            ? throw new ArgumentException("Content is required.", nameof(content))
            : content.Trim();
    }

    public void UpdateStatus(MessageStatus status)
    {
        Status = status;
    }
}
