using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakaleSistemi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeKullniciTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IlgiAlani",
                table: "Kullanicilar",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IlgiAlani",
                table: "Kullanicilar");
        }
    }
}
