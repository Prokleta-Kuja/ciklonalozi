using Microsoft.EntityFrameworkCore;

namespace ciklonalozi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Order> Orders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Order>(e =>
            {
                e.HasKey(p => p.OrderId);
            });
        }
    }
}