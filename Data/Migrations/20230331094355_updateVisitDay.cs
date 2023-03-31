using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class updateVisitDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "AdvertiseVisitRequests");

            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "AdvertiseAvailableVisitDays");

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableVisitDay",
                table: "AdvertiseVisitRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableVisitDay",
                table: "AdvertiseAvailableVisitDays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableVisitDay",
                table: "AdvertiseVisitRequests");

            migrationBuilder.DropColumn(
                name: "AvailableVisitDay",
                table: "AdvertiseAvailableVisitDays");

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "AdvertiseVisitRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "AdvertiseAvailableVisitDays",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
