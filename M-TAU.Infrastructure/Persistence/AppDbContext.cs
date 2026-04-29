using M_TAU.Domain.Catalog;
using M_TAU.Domain.Chat;
using M_TAU.Domain.Identity;
using M_TAU.Domain.Transaction;
using Microsoft.EntityFrameworkCore;

namespace M_TAU.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<TechnicalSpec> TechnicalSpecs => Set<TechnicalSpec>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
