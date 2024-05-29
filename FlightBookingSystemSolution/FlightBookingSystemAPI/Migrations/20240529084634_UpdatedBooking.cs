using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightBookingSystemAPI.Migrations
{
    public partial class UpdatedBooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PassengerCount",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassengerCount",
                table: "Bookings");
        }
    }
}
