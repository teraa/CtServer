using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CtServer.Data.Migrations
{
    public partial class AddPresentationPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_at",
                table: "presentations");

            migrationBuilder.DropColumn(
                name: "start_at",
                table: "presentations");

            migrationBuilder.AddColumn<int>(
                name: "position",
                table: "presentations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "position",
                table: "presentations");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "end_at",
                table: "presentations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "start_at",
                table: "presentations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
