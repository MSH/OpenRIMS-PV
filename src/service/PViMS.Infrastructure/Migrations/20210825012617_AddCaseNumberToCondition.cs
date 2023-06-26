using Microsoft.EntityFrameworkCore.Migrations;

namespace PViMS.Infrastructure.Migrations
{
    public partial class AddCaseNumberToCondition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CaseNumber",
                table: "PatientCondition",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaseNumber",
                table: "PatientCondition");
        }
    }
}
