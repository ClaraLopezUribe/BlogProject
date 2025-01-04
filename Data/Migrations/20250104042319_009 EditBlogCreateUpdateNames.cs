using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class _009EditBlogCreateUpdateNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Blogs",
                newName: "Updated");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Blogs",
                newName: "Created");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "Blogs",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Blogs",
                newName: "CreatedDate");
        }
    }
}
