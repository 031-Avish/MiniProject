using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightBookingSystemAPI.Migrations
{
    public partial class updateFlightandRoute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScheduleStatus",
                table: "Schedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RouteStatus",
                table: "RouteInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FlightStatus",
                table: "Flights",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduleStatus",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "RouteStatus",
                table: "RouteInfos");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FlightStatus",
                table: "Flights");
        }
    }
}
