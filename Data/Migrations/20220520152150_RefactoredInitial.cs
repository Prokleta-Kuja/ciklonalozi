using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ciklonalozi.Data.Migrations
{
    public partial class RefactoredInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContactName = table.Column<string>(type: "TEXT", nullable: false),
                    ContactPhone = table.Column<string>(type: "TEXT", nullable: true),
                    ContactPhoneNormalized = table.Column<string>(type: "TEXT", nullable: true),
                    Subject = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Arrival = table.Column<long>(type: "INTEGER", nullable: false),
                    Arrived = table.Column<long>(type: "INTEGER", nullable: true),
                    Completed = table.Column<long>(type: "INTEGER", nullable: true),
                    Returned = table.Column<long>(type: "INTEGER", nullable: true),
                    EstimatedPrice = table.Column<double>(type: "REAL", nullable: true),
                    RealPrice = table.Column<double>(type: "REAL", nullable: true),
                    Removed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
