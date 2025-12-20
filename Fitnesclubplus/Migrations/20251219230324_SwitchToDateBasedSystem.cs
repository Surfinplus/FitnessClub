using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitnesclubplus.Migrations
{
    /// <inheritdoc />
    public partial class SwitchToDateBasedSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "TrainerAvailabilities");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "TrainerAvailabilities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "TrainerAvailabilities");

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "TrainerAvailabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
