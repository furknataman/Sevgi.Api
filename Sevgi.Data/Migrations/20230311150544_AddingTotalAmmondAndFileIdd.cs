using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevgi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingTotalAmmondAndFileIdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalAmount",
                table: "Users",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Users");
        }
    }
}
