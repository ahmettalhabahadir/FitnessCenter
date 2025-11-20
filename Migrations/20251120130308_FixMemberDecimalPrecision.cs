using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessCenter.Migrations
{
    /// <inheritdoc />
    public partial class FixMemberDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_MemberId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainerServices_Services_ServicesId",
                table: "TrainerServices");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainerServices_Trainers_TrainersId",
                table: "TrainerServices");

            migrationBuilder.DropColumn(
                name: "AvailabilitySlots",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "TrainersId",
                table: "TrainerServices",
                newName: "ServiceId");

            migrationBuilder.RenameColumn(
                name: "ServicesId",
                table: "TrainerServices",
                newName: "TrainerId");

            migrationBuilder.RenameIndex(
                name: "IX_TrainerServices_TrainersId",
                table: "TrainerServices",
                newName: "IX_TrainerServices_ServiceId");

            migrationBuilder.RenameColumn(
                name: "Specialization",
                table: "Trainers",
                newName: "Specializations");

            migrationBuilder.RenameColumn(
                name: "DurationMinutes",
                table: "Services",
                newName: "DurationInMinutes");

            migrationBuilder.RenameColumn(
                name: "AppointmentDateTime",
                table: "Appointments",
                newName: "StartDateTime");

            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "Trainers",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Gyms",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkingHours",
                table: "Gyms",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "Appointments",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "GymId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAtBooking",
                table: "Appointments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Goals = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_AspNetUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainerAvailabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainerId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainerAvailabilities_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_GymId",
                table: "Appointments",
                column: "GymId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_IdentityUserId",
                table: "Members",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerAvailabilities_TrainerId",
                table: "TrainerAvailabilities",
                column: "TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Gyms_GymId",
                table: "Appointments",
                column: "GymId",
                principalTable: "Gyms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Members_MemberId",
                table: "Appointments",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerServices_Services_ServiceId",
                table: "TrainerServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerServices_Trainers_TrainerId",
                table: "TrainerServices",
                column: "TrainerId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Gyms_GymId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Members_MemberId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainerServices_Services_ServiceId",
                table: "TrainerServices");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainerServices_Trainers_TrainerId",
                table: "TrainerServices");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "TrainerAvailabilities");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_GymId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Biography",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Gyms");

            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "Gyms");

            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "GymId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PriceAtBooking",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "TrainerServices",
                newName: "TrainersId");

            migrationBuilder.RenameColumn(
                name: "TrainerId",
                table: "TrainerServices",
                newName: "ServicesId");

            migrationBuilder.RenameIndex(
                name: "IX_TrainerServices_ServiceId",
                table: "TrainerServices",
                newName: "IX_TrainerServices_TrainersId");

            migrationBuilder.RenameColumn(
                name: "Specializations",
                table: "Trainers",
                newName: "Specialization");

            migrationBuilder.RenameColumn(
                name: "DurationInMinutes",
                table: "Services",
                newName: "DurationMinutes");

            migrationBuilder.RenameColumn(
                name: "StartDateTime",
                table: "Appointments",
                newName: "AppointmentDateTime");

            migrationBuilder.AddColumn<string>(
                name: "AvailabilitySlots",
                table: "Trainers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MemberId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_MemberId",
                table: "Appointments",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerServices_Services_ServicesId",
                table: "TrainerServices",
                column: "ServicesId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerServices_Trainers_TrainersId",
                table: "TrainerServices",
                column: "TrainersId",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
