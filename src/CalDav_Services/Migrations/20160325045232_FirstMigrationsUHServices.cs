using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace CalDav_Services.Migrations
{
    public partial class FirstMigrationsUHServices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });
            migrationBuilder.CreateTable(
                name: "CalendarCollection",
                columns: table => new
                {
                    CalendarCollectionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CalendarDescription = table.Column<string>(nullable: true),
                    CreationDate = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    GetContentLanguage = table.Column<string>(nullable: true),
                    GetContentType = table.Column<string>(nullable: true),
                    GetEtag = table.Column<string>(nullable: true),
                    GetLastModified = table.Column<string>(nullable: true),
                    LockDiscovery = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ResourceType = table.Column<string>(nullable: true),
                    SupportedLock = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarCollection", x => x.CalendarCollectionId);
                    table.ForeignKey(
                        name: "FK_CalendarCollection_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "CalendarResource",
                columns: table => new
                {
                    CalendarResourceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CollectionCalendarCollectionId = table.Column<int>(nullable: true),
                    CreationDate = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: false),
                    GetContentLanguage = table.Column<string>(nullable: true),
                    GetContentLength = table.Column<string>(nullable: true),
                    GetContentType = table.Column<string>(nullable: true),
                    GetEtag = table.Column<string>(nullable: true),
                    GetLastModified = table.Column<string>(nullable: true),
                    LockDiscovery = table.Column<string>(nullable: true),
                    SupportedLock = table.Column<string>(nullable: true),
                    Uid = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarResource", x => x.CalendarResourceId);
                    table.ForeignKey(
                        name: "FK_CalendarResource_CalendarCollection_CollectionCalendarCollectionId",
                        column: x => x.CollectionCalendarCollectionId,
                        principalTable: "CalendarCollection",
                        principalColumn: "CalendarCollectionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalendarResource_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("CalendarResource");
            migrationBuilder.DropTable("CalendarCollection");
            migrationBuilder.DropTable("User");
        }
    }
}
