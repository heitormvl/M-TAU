using System.ComponentModel.DataAnnotations;
using M_TAU.Domain.Common;

namespace M_TAU.Domain.Transaction;

public class Feedback : EntityBase<Guid>
{
    [Required]
    [Range(1, 5)]
    public int Rating { get; private set; }

    [StringLength(1000)]
    public string? Comment { get; private set; }

    [Required]
    public Guid FromUserId { get; private set; }

    public Guid? OrderId { get; private set; }

    protected Feedback() { }

    public Feedback(Guid id, int rating, Guid fromUserId, Guid? orderId = null)
        : base(id)
    {
        SetRating(rating);
        FromUserId = fromUserId;
        OrderId = orderId;
    }

    public void SetRating(int rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));

        Rating = rating;
    }

    public void SetComment(string? comment)
    {
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
    }
}
