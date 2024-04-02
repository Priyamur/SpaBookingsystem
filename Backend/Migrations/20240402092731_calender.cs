using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class calender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "Calenders",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "Appointments",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CalenderId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CalenderId",
                table: "Appointments",
                column: "CalenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Calenders_CalenderId",
                table: "Appointments",
                column: "CalenderId",
                principalTable: "Calenders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Calenders_CalenderId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_CalenderId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CalenderId",
                table: "Appointments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Calenders",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "Date",
                table: "Appointments",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
