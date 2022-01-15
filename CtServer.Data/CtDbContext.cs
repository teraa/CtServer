using CtServer.Data.Models;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618
namespace CtServer.Data;

public class CtDbContext : DbContext
{
    public CtDbContext(DbContextOptions<CtDbContext> options)
        : base(options) { }

    public DbSet<Event> Events { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<Presentation> Presentations { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserEvent> UserEvents { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Photo> Photos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CtDbContext).Assembly);
    }
}
