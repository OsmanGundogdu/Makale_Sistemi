using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakaleSistemi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMakaleTable3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfYolu",
                table: "Makaleler");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfYolu",
                table: "Makaleler",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
