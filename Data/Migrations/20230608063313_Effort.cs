using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ciklonalozi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Effort : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Effort",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Effort",
                table: "Orders");
        }
    }
}
