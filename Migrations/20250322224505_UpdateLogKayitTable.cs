using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakaleSistemi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLogKayitTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Aciklama",
                table: "LogKayitlari",
                newName: "Islem");

            migrationBuilder.AddColumn<int>(
                name: "KullaniciId",
                table: "LogKayitlari",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MakaleId",
                table: "LogKayitlari",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LogKayitlari_KullaniciId",
                table: "LogKayitlari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_LogKayitlari_MakaleId",
                table: "LogKayitlari",
                column: "MakaleId");

            migrationBuilder.AddForeignKey(
                name: "FK_LogKayitlari_Kullanicilar_KullaniciId",
                table: "LogKayitlari",
                column: "KullaniciId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LogKayitlari_Makaleler_MakaleId",
                table: "LogKayitlari",
                column: "MakaleId",
                principalTable: "Makaleler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogKayitlari_Kullanicilar_KullaniciId",
                table: "LogKayitlari");

            migrationBuilder.DropForeignKey(
                name: "FK_LogKayitlari_Makaleler_MakaleId",
                table: "LogKayitlari");

            migrationBuilder.DropIndex(
                name: "IX_LogKayitlari_KullaniciId",
                table: "LogKayitlari");

            migrationBuilder.DropIndex(
                name: "IX_LogKayitlari_MakaleId",
                table: "LogKayitlari");

            migrationBuilder.DropColumn(
                name: "KullaniciId",
                table: "LogKayitlari");

            migrationBuilder.DropColumn(
                name: "MakaleId",
                table: "LogKayitlari");

            migrationBuilder.RenameColumn(
                name: "Islem",
                table: "LogKayitlari",
                newName: "Aciklama");
        }
    }
}
