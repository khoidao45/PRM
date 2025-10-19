using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVStation_basedRentalSysteEM.Services.BookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class updatebookinfa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "HopDongId",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HopDongId",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
