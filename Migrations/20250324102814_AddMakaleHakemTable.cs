using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakaleSistemi.Migrations
{
    /// <inheritdoc />
    public partial class AddMakaleHakemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MakaleHakemler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MakaleId = table.Column<int>(type: "INTEGER", nullable: false),
                    HakemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Durum = table.Column<string>(type: "TEXT", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MakaleHakemler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MakaleHakemler_Kullanicilar_HakemId",
                        column: x => x.HakemId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MakaleHakemler_Makaleler_MakaleId",
                        column: x => x.MakaleId,
                        principalTable: "Makaleler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MakaleHakemler_HakemId",
                table: "MakaleHakemler",
                column: "HakemId");

            migrationBuilder.CreateIndex(
                name: "IX_MakaleHakemler_MakaleId",
                table: "MakaleHakemler",
                column: "MakaleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MakaleHakemler");
        }
    }
}
