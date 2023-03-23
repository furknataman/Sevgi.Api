using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sevgi.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeleteTotalAmmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalAmount",
                table: "Users",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
