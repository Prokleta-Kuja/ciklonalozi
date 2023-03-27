using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ciklonalozi.Data
{
    public class AppDbContext : DbContext, IDataProtectionKeyContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Order>(e =>
            {
                e.HasKey(p => p.OrderId);
            });

            // SQLite conversions
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var dtProperties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

                foreach (var property in dtProperties)
                    builder.Entity(entityType.Name).Property(property.Name).HasConversion(new DateTimeToBinaryConverter());

                var decProperties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));

                foreach (var property in decProperties)
                    builder.Entity(entityType.Name).Property(property.Name).HasConversion<double>();

                var spanProperties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(TimeSpan) || p.PropertyType == typeof(TimeSpan?));

                foreach (var property in spanProperties)
                    builder.Entity(entityType.Name).Property(property.Name).HasConversion<long>();
            }
        }

        internal async Task DemoDataAsync()
        {
            var now = DateTime.UtcNow;
            Orders.Add(new("Jučer", null, "Subjekt a", now)
            {
                Arrived = now.AddMinutes(5),
                Removed = true,
            });
            Orders.Add(new("Danas", null, "Subjekt b", now)
            {
                Arrived = now.AddMinutes(5),
            });

            // TODO: dodaj još

            await SaveChangesAsync();
        }
    }
}