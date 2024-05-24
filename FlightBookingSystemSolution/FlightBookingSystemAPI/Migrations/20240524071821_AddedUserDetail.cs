using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightBookingSystemAPI.Migrations
{
    public partial class AddedUserDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingsDetails_Bookings_BookingId",
                table: "BookingsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingsDetails_Passengers_PassengerId",
                table: "BookingsDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingsDetails",
                table: "BookingsDetails");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "BookingsDetails",
                newName: "BookingDetails");

            migrationBuilder.RenameIndex(
                name: "IX_BookingsDetails_PassengerId",
                table: "BookingDetails",
                newName: "IX_BookingDetails_PassengerId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingsDetails_BookingId",
                table: "BookingDetails",
                newName: "IX_BookingDetails_BookingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingDetails",
                table: "BookingDetails",
                column: "BookingDetailId");

            migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordHashKey = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserDetails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetails_Bookings_BookingId",
                table: "BookingDetails",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetails_Passengers_PassengerId",
                table: "BookingDetails",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "PassengerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetails_Bookings_BookingId",
                table: "BookingDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetails_Passengers_PassengerId",
                table: "BookingDetails");

            migrationBuilder.DropTable(
                name: "UserDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookingDetails",
                table: "BookingDetails");

            migrationBuilder.RenameTable(
                name: "BookingDetails",
                newName: "BookingsDetails");

            migrationBuilder.RenameIndex(
                name: "IX_BookingDetails_PassengerId",
                table: "BookingsDetails",
                newName: "IX_BookingsDetails_PassengerId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingDetails_BookingId",
                table: "BookingsDetails",
                newName: "IX_BookingsDetails_BookingId");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookingsDetails",
                table: "BookingsDetails",
                column: "BookingDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingsDetails_Bookings_BookingId",
                table: "BookingsDetails",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingsDetails_Passengers_PassengerId",
                table: "BookingsDetails",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "PassengerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
