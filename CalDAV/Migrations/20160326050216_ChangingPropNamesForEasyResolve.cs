using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace CalDAV.Migrations
{
    public partial class ChangingPropNamesForEasyResolve : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_CalendarCollection_User_UserId", table: "CalendarCollection");
            migrationBuilder.DropForeignKey(name: "FK_CalendarResource_User_UserId", table: "CalendarResource");
            migrationBuilder.AddForeignKey(
                name: "FK_CalendarCollection_User_UserId",
                table: "CalendarCollection",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_CalendarResource_User_UserId",
                table: "CalendarResource",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.RenameColumn(
                name: "SupportedLock",
                table: "CalendarResource",
                newName: "Supportedlock");
            migrationBuilder.RenameColumn(
                name: "LockDiscovery",
                table: "CalendarResource",
                newName: "Lockdiscovery");
            migrationBuilder.RenameColumn(
                name: "GetLastModified",
                table: "CalendarResource",
                newName: "Getlastmodified");
            migrationBuilder.RenameColumn(
                name: "GetEtag",
                table: "CalendarResource",
                newName: "Getetag");
            migrationBuilder.RenameColumn(
                name: "GetContentType",
                table: "CalendarResource",
                newName: "Getcontenttype");
            migrationBuilder.RenameColumn(
                name: "GetContentLength",
                table: "CalendarResource",
                newName: "Getcontentlength");
            migrationBuilder.RenameColumn(
                name: "GetContentLanguage",
                table: "CalendarResource",
                newName: "Getcontentlanguage");
            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "CalendarResource",
                newName: "Displayname");
            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "CalendarResource",
                newName: "Creationdate");
            migrationBuilder.RenameColumn(
                name: "SupportedLock",
                table: "CalendarCollection",
                newName: "Supportedlock");
            migrationBuilder.RenameColumn(
                name: "ResourceType",
                table: "CalendarCollection",
                newName: "Resourcetype");
            migrationBuilder.RenameColumn(
                name: "LockDiscovery",
                table: "CalendarCollection",
                newName: "Lockdiscovery");
            migrationBuilder.RenameColumn(
                name: "GetLastModified",
                table: "CalendarCollection",
                newName: "Getlastmodified");
            migrationBuilder.RenameColumn(
                name: "GetEtag",
                table: "CalendarCollection",
                newName: "Getetag");
            migrationBuilder.RenameColumn(
                name: "GetContentType",
                table: "CalendarCollection",
                newName: "GetContenttype");
            migrationBuilder.RenameColumn(
                name: "GetContentLanguage",
                table: "CalendarCollection",
                newName: "Getcontentlanguage");
            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "CalendarCollection",
                newName: "Displayname");
            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "CalendarCollection",
                newName: "Creationdate");
            migrationBuilder.RenameColumn(
                name: "CalendarDescription",
                table: "CalendarCollection",
                newName: "Calendardescription");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_CalendarCollection_User_UserId", table: "CalendarCollection");
            migrationBuilder.DropForeignKey(name: "FK_CalendarResource_User_UserId", table: "CalendarResource");
            migrationBuilder.AddForeignKey(
                name: "FK_CalendarCollection_User_UserId",
                table: "CalendarCollection",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_CalendarResource_User_UserId",
                table: "CalendarResource",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.RenameColumn(
                name: "Supportedlock",
                table: "CalendarResource",
                newName: "SupportedLock");
            migrationBuilder.RenameColumn(
                name: "Lockdiscovery",
                table: "CalendarResource",
                newName: "LockDiscovery");
            migrationBuilder.RenameColumn(
                name: "Getlastmodified",
                table: "CalendarResource",
                newName: "GetLastModified");
            migrationBuilder.RenameColumn(
                name: "Getetag",
                table: "CalendarResource",
                newName: "GetEtag");
            migrationBuilder.RenameColumn(
                name: "Getcontenttype",
                table: "CalendarResource",
                newName: "GetContentType");
            migrationBuilder.RenameColumn(
                name: "Getcontentlength",
                table: "CalendarResource",
                newName: "GetContentLength");
            migrationBuilder.RenameColumn(
                name: "Getcontentlanguage",
                table: "CalendarResource",
                newName: "GetContentLanguage");
            migrationBuilder.RenameColumn(
                name: "Displayname",
                table: "CalendarResource",
                newName: "DisplayName");
            migrationBuilder.RenameColumn(
                name: "Creationdate",
                table: "CalendarResource",
                newName: "CreationDate");
            migrationBuilder.RenameColumn(
                name: "Supportedlock",
                table: "CalendarCollection",
                newName: "SupportedLock");
            migrationBuilder.RenameColumn(
                name: "Resourcetype",
                table: "CalendarCollection",
                newName: "ResourceType");
            migrationBuilder.RenameColumn(
                name: "Lockdiscovery",
                table: "CalendarCollection",
                newName: "LockDiscovery");
            migrationBuilder.RenameColumn(
                name: "Getlastmodified",
                table: "CalendarCollection",
                newName: "GetLastModified");
            migrationBuilder.RenameColumn(
                name: "Getetag",
                table: "CalendarCollection",
                newName: "GetEtag");
            migrationBuilder.RenameColumn(
                name: "Getcontentlanguage",
                table: "CalendarCollection",
                newName: "GetContentLanguage");
            migrationBuilder.RenameColumn(
                name: "GetContenttype",
                table: "CalendarCollection",
                newName: "GetContentType");
            migrationBuilder.RenameColumn(
                name: "Displayname",
                table: "CalendarCollection",
                newName: "DisplayName");
            migrationBuilder.RenameColumn(
                name: "Creationdate",
                table: "CalendarCollection",
                newName: "CreationDate");
            migrationBuilder.RenameColumn(
                name: "Calendardescription",
                table: "CalendarCollection",
                newName: "CalendarDescription");
        }
    }
}
