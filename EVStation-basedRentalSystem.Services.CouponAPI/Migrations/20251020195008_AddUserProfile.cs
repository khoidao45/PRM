using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVStation_basedRentalSystem.Services.AuthAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "StaffProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RenterProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AdminProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StaffProfiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RenterProfiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AdminProfiles");
        }
    }
}
