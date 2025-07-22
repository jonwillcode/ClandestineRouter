using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClandestineRouter.Migrations
{
    /// <inheritdoc />
    public partial class IdentityUserLocalTimeZone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocalTimeZone",
                table: "AspNetUsers",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalTimeZone",
                table: "AspNetUsers");
        }
    }
}
