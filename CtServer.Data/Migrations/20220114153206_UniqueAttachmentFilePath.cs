using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CtServer.Data.Migrations
{
    public partial class UniqueAttachmentFilePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_attachments_file_path",
                table: "attachments",
                column: "file_path",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_attachments_file_path",
                table: "attachments");
        }
    }
}
