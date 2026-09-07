using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IronGyms.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressLine",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "Profiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "Profiles");
        }
    }
}
