using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CtServer.Data.Migrations
{
    public partial class AddLocationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "location",
                table: "sections");

            migrationBuilder.AddColumn<int>(
                name: "location_id",
                table: "sections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locations", x => x.id);
                    table.ForeignKey(
                        name: "fk_locations_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sections_location_id",
                table: "sections",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_locations_event_id",
                table: "locations",
                column: "event_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sections_locations_location_id",
                table: "sections",
                column: "location_id",
                principalTable: "locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sections_locations_location_id",
                table: "sections");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropIndex(
                name: "ix_sections_location_id",
                table: "sections");

            migrationBuilder.DropColumn(
                name: "location_id",
                table: "sections");

            migrationBuilder.AddColumn<string>(
                name: "location",
                table: "sections",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
