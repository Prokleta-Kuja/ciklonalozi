using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ciklonalozi.Data.Migrations
{
    /// <inheritdoc />
    public partial class NormalizationsFill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var opt = new DbContextOptionsBuilder<AppDbContext>();
            opt.UseSqlite(C.Settings.AppDbConnectionString);
            using var db = new AppDbContext(opt.Options);

            foreach (var order in db.Orders)
            {
                order.ContactNameNormalized = C.Normalize(order.ContactName);
                if (!string.IsNullOrWhiteSpace(order.Description))
                    order.DescriptionNormalized = C.Normalize(order.Description);
            }
            db.SaveChanges();
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
