using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClandestineRouter.Migrations
{
    /// <inheritdoc />
    public partial class AddSocialMediaAppIdToSocialMedia2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppId",
                table: "SocialMediaAccounts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppId",
                table: "SocialMediaAccounts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
