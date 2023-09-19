using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ciklonalozi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Normalizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactNameNormalized",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionNormalized",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNameNormalized",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DescriptionNormalized",
                table: "Orders");
        }
    }
}
