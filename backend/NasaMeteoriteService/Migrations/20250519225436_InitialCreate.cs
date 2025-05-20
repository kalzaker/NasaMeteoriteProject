using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NasaMeteoriteService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Meteorites",
                columns: table => new
                {
                    MeteoriteId = table.Column<string>(type: "text", nullable: false),
                    NasaId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Nametype = table.Column<string>(type: "text", nullable: false),
                    Recclass = table.Column<string>(type: "text", nullable: false),
                    Mass = table.Column<float>(type: "real", nullable: true),
                    Fall = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reclat = table.Column<float>(type: "real", nullable: true),
                    Reclong = table.Column<float>(type: "real", nullable: true),
                    GeoLat = table.Column<double>(type: "double precision", nullable: true),
                    GeoLong = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meteorites", x => x.MeteoriteId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meteorites_NasaId",
                table: "Meteorites",
                column: "NasaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Meteorites");
        }
    }
}
