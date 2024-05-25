using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightBookingSystemAPI.Migrations
{
    public partial class updatedUserDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserDetails");
        }
    }
}
