using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ciklonalozi.Data.Migrations
{
    public partial class Push : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Auth",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Endpoint",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "P256DH",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Auth",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Endpoint",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "P256DH",
                table: "Orders");
        }
    }
}
