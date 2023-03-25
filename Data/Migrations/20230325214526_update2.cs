using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuildingType",
                table: "UserAdvertises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "DespositPrice",
                table: "UserAdvertises",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "ForSale",
                table: "UserAdvertises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasGarage",
                table: "UserAdvertises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "RentPrice",
                table: "UserAdvertises",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingType",
                table: "UserAdvertises");

            migrationBuilder.DropColumn(
                name: "DespositPrice",
                table: "UserAdvertises");

            migrationBuilder.DropColumn(
                name: "ForSale",
                table: "UserAdvertises");

            migrationBuilder.DropColumn(
                name: "HasGarage",
                table: "UserAdvertises");

            migrationBuilder.DropColumn(
                name: "RentPrice",
                table: "UserAdvertises");
        }
    }
}
