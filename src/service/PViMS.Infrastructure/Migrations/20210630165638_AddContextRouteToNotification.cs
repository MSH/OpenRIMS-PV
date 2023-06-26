using Microsoft.EntityFrameworkCore.Migrations;

namespace PViMS.Infrastructure.Migrations
{
    public partial class AddContextRouteToNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContextId",
                table: "Notification");

            migrationBuilder.AddColumn<string>(
                name: "ContextRoute",
                table: "Notification",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContextRoute",
                table: "Notification");

            migrationBuilder.AddColumn<int>(
                name: "ContextId",
                table: "Notification",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
