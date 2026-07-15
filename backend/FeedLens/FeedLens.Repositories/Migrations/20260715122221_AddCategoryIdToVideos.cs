using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FeedLens.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIdToVideos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Videos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Icon", "IsActive", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "💻", true, "Technology", 1 },
                    { 2, "💰", true, "Finance", 2 },
                    { 3, "📚", true, "Education", 3 },
                    { 4, "🎬", true, "Entertainment", 4 },
                    { 5, "⚽", true, "Sports", 5 },
                    { 6, "🎵", true, "Music", 6 },
                    { 7, "🎮", true, "Gaming", 7 },
                    { 8, "✈️", true, "Travel", 8 },
                    { 9, "🍕", true, "Food", 9 },
                    { 10, "🏥", true, "Health", 10 },
                    { 11, "🔬", true, "Science", 11 },
                    { 12, "💼", true, "Business", 12 },
                    { 13, "📰", true, "News", 13 },
                    { 14, "😂", true, "Comedy", 14 },
                    { 15, "💪", true, "Fitness", 15 },
                    { 16, "🎨", true, "Art & Design", 16 },
                    { 17, "📷", true, "Photography", 17 },
                    { 18, "👨‍💻", true, "Programming", 18 },
                    { 19, "🌟", true, "Lifestyle", 19 },
                    { 20, "📌", true, "Other", 20 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_CategoryId",
                table: "Videos",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Categories_CategoryId",
                table: "Videos",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Categories_CategoryId",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Videos_CategoryId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Videos");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
