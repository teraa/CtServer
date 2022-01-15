using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CtServer.Data.Migrations
{
    public partial class SubscriptionEndpointUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_endpoint",
                table: "subscriptions",
                column: "endpoint",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_subscriptions_endpoint",
                table: "subscriptions");
        }
    }
}
