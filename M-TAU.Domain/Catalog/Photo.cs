using System.ComponentModel.DataAnnotations;
using M_TAU.Domain.Common;

namespace M_TAU.Domain.Catalog;

public class Photo : EntityBase<Guid>
{
    [Required]
    [Url]
    public string Url { get; private set; } = string.Empty;

    [Required]
    public bool IsMain { get; private set; }

    protected Photo() { }

    public Photo(Guid id, string url, bool isMain = false)
        : base(id)
    {
        SetUrl(url);
        IsMain = isMain;
    }

    public void SetUrl(string url)
    {
        Url = string.IsNullOrWhiteSpace(url)
            ? throw new ArgumentException("URL is required.", nameof(url))
            : url.Trim();
    }

    public void SetAsMain(bool isMain)
    {
        IsMain = isMain;
    }
}
