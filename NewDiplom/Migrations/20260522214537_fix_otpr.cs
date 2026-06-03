using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewDiplom.Migrations
{
    /// <inheritdoc />
    public partial class fix_otpr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfirmationCode",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Shipments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RecipientPhone",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmationCode",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "RecipientPhone",
                table: "Shipments");
        }
    }
}
