using Microsoft.EntityFrameworkCore;

namespace ciklonalozi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Contact> Contacts { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Contact>(e =>
            {
                e.HasKey(p => p.ContactId);
            });

            builder.Entity<Order>(e =>
            {
                e.HasKey(p => p.OrderId);
                e.HasMany(p => p.Items).WithOne(p => p.Order!).OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<OrderItem>(e =>
            {
                e.HasKey(p => p.OrderItemId);
            });
        }
    }
}