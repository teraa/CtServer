using Microsoft.EntityFrameworkCore.Migrations;

namespace CtServer.Data.Migrations
{
    public partial class AddPresentationDuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "duration",
                table: "presentations",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "duration",
                table: "presentations");
        }
    }
}
