using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakaleSistemi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMakaleHakemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MakaleHakemler_Kullanicilar_HakemId",
                table: "MakaleHakemler");

            migrationBuilder.DropIndex(
                name: "IX_MakaleHakemler_HakemId",
                table: "MakaleHakemler");

            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "MakaleHakemler");

            migrationBuilder.AddColumn<int>(
                name: "Puan",
                table: "MakaleHakemler",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Tarih",
                table: "MakaleHakemler",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Yorum",
                table: "MakaleHakemler",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Puan",
                table: "MakaleHakemler");

            migrationBuilder.DropColumn(
                name: "Tarih",
                table: "MakaleHakemler");

            migrationBuilder.DropColumn(
                name: "Yorum",
                table: "MakaleHakemler");

            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "MakaleHakemler",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MakaleHakemler_HakemId",
                table: "MakaleHakemler",
                column: "HakemId");

            migrationBuilder.AddForeignKey(
                name: "FK_MakaleHakemler_Kullanicilar_HakemId",
                table: "MakaleHakemler",
                column: "HakemId",
                principalTable: "Kullanicilar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
