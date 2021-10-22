using CtServer.Data.Models;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618
namespace CtServer.Data
{
    public class CtDbContext : DbContext
    {
        public CtDbContext(DbContextOptions<CtDbContext> options)
            : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Presentation> Presentations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CtDbContext).Assembly);
        }
    }
}
