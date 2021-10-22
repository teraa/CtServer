using Microsoft.EntityFrameworkCore;

namespace CtServer.Data
{
    public class CtDbContext : DbContext
    {
        public CtDbContext(DbContextOptions<CtDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CtDbContext).Assembly);
        }
    }
}
