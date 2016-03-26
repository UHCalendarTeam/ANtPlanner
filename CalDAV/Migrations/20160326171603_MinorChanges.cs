using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace CalDAV.Migrations
{
    public partial class MinorChanges : Migration
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
                name: "GetContenttype",
                table: "CalendarCollection",
                newName: "Getcontenttype");
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
                name: "Getcontenttype",
                table: "CalendarCollection",
                newName: "GetContenttype");
        }
    }
}
