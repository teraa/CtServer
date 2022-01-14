using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CtServer.Data.Migrations
{
    public partial class AddPhotoModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "main_author_photo",
                table: "presentations");

            migrationBuilder.AddColumn<int>(
                name: "photo_id",
                table: "presentations",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "photos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    presentation_id = table.Column<int>(type: "integer", nullable: false),
                    file_path = table.Column<string>(type: "text", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_photos", x => x.id);
                    table.ForeignKey(
                        name: "fk_photos_presentations_presentation_id1",
                        column: x => x.presentation_id,
                        principalTable: "presentations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_photos_file_path",
                table: "photos",
                column: "file_path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_photos_presentation_id",
                table: "photos",
                column: "presentation_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "photos");

            migrationBuilder.DropColumn(
                name: "photo_id",
                table: "presentations");

            migrationBuilder.AddColumn<string>(
                name: "main_author_photo",
                table: "presentations",
                type: "text",
                nullable: true);
        }
    }
}
