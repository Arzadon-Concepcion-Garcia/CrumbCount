using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrumbCount2.Migrations
{
    /// <inheritdoc />
    public partial class AddDisposition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dispositions",
                columns: table => new
                {
                    DispositionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WasteRecordingId = table.Column<int>(type: "INTEGER", nullable: false),
                    DispositionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    DispositionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Remarks = table.Column<string>(type: "TEXT", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositions", x => x.DispositionId);
                    table.ForeignKey(
                        name: "FK_Dispositions_WasteRecordings_WasteRecordingId",
                        column: x => x.WasteRecordingId,
                        principalTable: "WasteRecordings",
                        principalColumn: "WasteRecordingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_WasteRecordingId",
                table: "Dispositions",
                column: "WasteRecordingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dispositions");
        }
    }
}
