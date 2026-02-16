using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsExplorerApp.Migrations
{
    /// <inheritdoc />
    public partial class FixFavoritesUrlColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ArticleUrl",
                table: "FavoriteArticles",
                newName: "Url");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteArticles_UserId_ArticleUrl",
                table: "FavoriteArticles",
                newName: "IX_FavoriteArticles_UserId_Url");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "FavoriteArticles",
                newName: "ArticleUrl");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteArticles_UserId_Url",
                table: "FavoriteArticles",
                newName: "IX_FavoriteArticles_UserId_ArticleUrl");
        }
    }
}
