using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CtServer.Data.Migrations
{
    public partial class RemovePresentationPhotoAttachmentIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attachments_presentations_presentation_id1",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_photos_presentations_presentation_id1",
                table: "photos");

            migrationBuilder.DropColumn(
                name: "attachment_id",
                table: "presentations");

            migrationBuilder.DropColumn(
                name: "photo_id",
                table: "presentations");

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_presentations_presentation_id",
                table: "attachments",
                column: "presentation_id",
                principalTable: "presentations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_photos_presentations_presentation_id",
                table: "photos",
                column: "presentation_id",
                principalTable: "presentations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attachments_presentations_presentation_id",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_photos_presentations_presentation_id",
                table: "photos");

            migrationBuilder.AddColumn<int>(
                name: "attachment_id",
                table: "presentations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "photo_id",
                table: "presentations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_presentations_presentation_id1",
                table: "attachments",
                column: "presentation_id",
                principalTable: "presentations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_photos_presentations_presentation_id1",
                table: "photos",
                column: "presentation_id",
                principalTable: "presentations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
