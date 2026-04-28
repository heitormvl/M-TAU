using System.ComponentModel.DataAnnotations;
using M_TAU.Domain.Common;

namespace M_TAU.Domain.Identity;

public class User : EntityBase<Guid>
{
    [Required]
    [StringLength(120)]
    public string Name { get; private set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; private set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string PasswordHash { get; private set; } = string.Empty;

    [Required]
    public UserType Role { get; private set; }

    [Required]
    public DateTime CreatedAt { get; private set; }

    protected User() { }

    public User(Guid id, string name, string email, string passwordHash, UserType role)
        : base(id)
    {
        SetName(name);
        SetEmail(email);
        PasswordHash = string.IsNullOrWhiteSpace(passwordHash)
            ? throw new ArgumentException("Password hash is required.", nameof(passwordHash))
            : passwordHash;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetName(string name)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Name is required.", nameof(name))
            : name.Trim();
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (!email.Contains("@"))
            throw new ArgumentException("Email must be valid.", nameof(email));

        Email = email.Trim().ToLowerInvariant();
    }
}
