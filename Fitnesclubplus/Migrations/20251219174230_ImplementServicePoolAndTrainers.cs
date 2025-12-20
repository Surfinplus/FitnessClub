using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitnesclubplus.Migrations
{
    /// <inheritdoc />
    public partial class ImplementServicePoolAndTrainers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_Services_ServiceId",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Trainers",
                newName: "GymId");

            migrationBuilder.RenameIndex(
                name: "IX_Trainers_ServiceId",
                table: "Trainers",
                newName: "IX_Trainers_GymId");

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "Trainers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceCategoryId",
                table: "Services",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrainerId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceCategoryId",
                table: "Services",
                column: "ServiceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_TrainerId",
                table: "Services",
                column: "TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceCategories_ServiceCategoryId",
                table: "Services",
                column: "ServiceCategoryId",
                principalTable: "ServiceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Trainers_TrainerId",
                table: "Services",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_Gyms_GymId",
                table: "Trainers",
                column: "GymId",
                principalTable: "Gyms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceCategories_ServiceCategoryId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Trainers_TrainerId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_Gyms_GymId",
                table: "Trainers");

            migrationBuilder.DropTable(
                name: "ServiceCategories");

            migrationBuilder.DropIndex(
                name: "IX_Services_ServiceCategoryId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_TrainerId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "ServiceCategoryId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TrainerId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "GymId",
                table: "Trainers",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Trainers_GymId",
                table: "Trainers",
                newName: "IX_Trainers_ServiceId");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Trainers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_Services_ServiceId",
                table: "Trainers",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
