using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Required : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirm",
                table: "UserAdvertises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AdvertiseAvailableVisitDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdvertiseId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertiseAvailableVisitDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvertiseAvailableVisitDays_UserAdvertises_AdvertiseId",
                        column: x => x.AdvertiseId,
                        principalTable: "UserAdvertises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvertiseAvailableVisitDays_AdvertiseId",
                table: "AdvertiseAvailableVisitDays",
                column: "AdvertiseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertiseAvailableVisitDays");

            migrationBuilder.DropColumn(
                name: "IsConfirm",
                table: "UserAdvertises");
        }
    }
}
