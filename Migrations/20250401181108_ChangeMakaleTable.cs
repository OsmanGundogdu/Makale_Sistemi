using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakaleSistemi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMakaleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Konu",
                table: "Makaleler",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Konu",
                table: "Makaleler");
        }
    }
}
