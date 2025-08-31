using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClandestineRouter.Migrations
{
    /// <inheritdoc />
    public partial class BigUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "SocialMediaApps",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "SocialMediaApps",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "SocialMediaAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SocialMediaAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "SocialMediaAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "SocialMediaAccountLinks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SocialMediaAccountLinks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "SocialMediaAccountLinks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Personas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Personas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "Personas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "PersonaAssociations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PersonaAssociations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "PersonaAssociations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "InboundContents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "InboundContents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "InboundContents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "EncounterTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "EncounterTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Encounters",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Encounters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "Encounters",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OccurrenceDateTimeUtc",
                table: "Encounters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "BehaviorTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedById",
                table: "BehaviorTypes",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "SocialMediaApps");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "SocialMediaApps");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "SocialMediaAccounts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SocialMediaAccounts");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "SocialMediaAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "SocialMediaAccountLinks");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SocialMediaAccountLinks");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "SocialMediaAccountLinks");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Personas");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "PersonaAssociations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PersonaAssociations");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "PersonaAssociations");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "InboundContents");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "InboundContents");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "InboundContents");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "EncounterTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "EncounterTypes");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Encounters");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Encounters");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Encounters");

            migrationBuilder.DropColumn(
                name: "OccurrenceDateTimeUtc",
                table: "Encounters");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "BehaviorTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "BehaviorTypes");
        }
    }
}
