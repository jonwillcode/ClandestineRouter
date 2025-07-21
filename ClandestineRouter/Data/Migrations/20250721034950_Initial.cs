using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClandestineRouter.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BehaviorTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehaviorTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EncounterTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncounterTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocialMediaApps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialMediaApps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InboundContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExtractedText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboundContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboundContents_Personas_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonaAssociations",
                columns: table => new
                {
                    BasePersonaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssociatePersonaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonaAssociations", x => new { x.BasePersonaId, x.AssociatePersonaId });
                    table.ForeignKey(
                        name: "FK_PersonaAssociations_Personas_AssociatePersonaId",
                        column: x => x.AssociatePersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PersonaAssociations_Personas_BasePersonaId",
                        column: x => x.BasePersonaId,
                        principalTable: "Personas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SocialMediaAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SocialMediaAppId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialMediaAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialMediaAccounts_SocialMediaApps_SocialMediaAppId",
                        column: x => x.SocialMediaAppId,
                        principalTable: "SocialMediaApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Encounters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SocialMediaAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EncounterTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Encounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Encounters_EncounterTypes_EncounterTypeId",
                        column: x => x.EncounterTypeId,
                        principalTable: "EncounterTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Encounters_SocialMediaAccounts_SocialMediaAccountId",
                        column: x => x.SocialMediaAccountId,
                        principalTable: "SocialMediaAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialMediaAccountLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SocialMediaAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialMediaAccountLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialMediaAccountLinks_SocialMediaAccounts_SocialMediaAccountId",
                        column: x => x.SocialMediaAccountId,
                        principalTable: "SocialMediaAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EncounterBeginBehaviorType",
                columns: table => new
                {
                    BeginBehaviorTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EncountersBeginId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncounterBeginBehaviorType", x => new { x.BeginBehaviorTypeId, x.EncountersBeginId });
                    table.ForeignKey(
                        name: "FK_EncounterBeginBehaviorType_BehaviorTypes_BeginBehaviorTypeId",
                        column: x => x.BeginBehaviorTypeId,
                        principalTable: "BehaviorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EncounterBeginBehaviorType_Encounters_EncountersBeginId",
                        column: x => x.EncountersBeginId,
                        principalTable: "Encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EncounterEndBehaviorType",
                columns: table => new
                {
                    EncountersEndId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndBehaviorTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EncounterEndBehaviorType", x => new { x.EncountersEndId, x.EndBehaviorTypeId });
                    table.ForeignKey(
                        name: "FK_EncounterEndBehaviorType_BehaviorTypes_EndBehaviorTypeId",
                        column: x => x.EndBehaviorTypeId,
                        principalTable: "BehaviorTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EncounterEndBehaviorType_Encounters_EncountersEndId",
                        column: x => x.EncountersEndId,
                        principalTable: "Encounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EncounterBeginBehaviorType_EncountersBeginId",
                table: "EncounterBeginBehaviorType",
                column: "EncountersBeginId");

            migrationBuilder.CreateIndex(
                name: "IX_EncounterEndBehaviorType_EndBehaviorTypeId",
                table: "EncounterEndBehaviorType",
                column: "EndBehaviorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_EncounterTypeId",
                table: "Encounters",
                column: "EncounterTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Encounters_SocialMediaAccountId",
                table: "Encounters",
                column: "SocialMediaAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundContents_PersonaId",
                table: "InboundContents",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonaAssociations_AssociatePersonaId",
                table: "PersonaAssociations",
                column: "AssociatePersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonaAssociations_BasePersonaId_AssociatePersonaId",
                table: "PersonaAssociations",
                columns: new[] { "BasePersonaId", "AssociatePersonaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SocialMediaAccountLinks_SocialMediaAccountId",
                table: "SocialMediaAccountLinks",
                column: "SocialMediaAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialMediaAccounts_SocialMediaAppId",
                table: "SocialMediaAccounts",
                column: "SocialMediaAppId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EncounterBeginBehaviorType");

            migrationBuilder.DropTable(
                name: "EncounterEndBehaviorType");

            migrationBuilder.DropTable(
                name: "InboundContents");

            migrationBuilder.DropTable(
                name: "PersonaAssociations");

            migrationBuilder.DropTable(
                name: "SocialMediaAccountLinks");

            migrationBuilder.DropTable(
                name: "BehaviorTypes");

            migrationBuilder.DropTable(
                name: "Encounters");

            migrationBuilder.DropTable(
                name: "Personas");

            migrationBuilder.DropTable(
                name: "EncounterTypes");

            migrationBuilder.DropTable(
                name: "SocialMediaAccounts");

            migrationBuilder.DropTable(
                name: "SocialMediaApps");
        }
    }
}
