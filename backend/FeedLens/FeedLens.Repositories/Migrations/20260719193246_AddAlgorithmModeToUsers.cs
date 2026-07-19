using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedLens.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddAlgorithmModeToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlgorithmMode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlgorithmMode",
                table: "Users");
        }
    }
}
