using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CtServer.Data.Migrations
{
    public partial class AddAttachmentFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "file_name",
                table: "attachments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_name",
                table: "attachments");
        }
    }
}
