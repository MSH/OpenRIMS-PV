using Microsoft.EntityFrameworkCore.Migrations;

namespace PViMS.Infrastructure.Migrations
{
    public partial class AddReportClassification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReportClassificationId",
                table: "ReportInstance",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportClassificationId",
                table: "ReportInstance");
        }
    }
}
