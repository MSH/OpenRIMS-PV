using Microsoft.EntityFrameworkCore.Migrations;

namespace PViMS.Infrastructure.Migrations
{
    public partial class FixUniqueIndexOnDatasetInstanceSubValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_DatasetInstanceSubValue_DatasetInstanceValue_Id_DatasetElementSub_Id",
            //    table: "DatasetInstanceSubValue");

            //migrationBuilder.CreateIndex(
            //    name: "IX_DatasetInstanceSubValue_ContextValue_DatasetInstanceValue_Id_DatasetElementSub_Id",
            //    table: "DatasetInstanceSubValue",
            //    columns: new[] { "ContextValue", "DatasetInstanceValue_Id", "DatasetElementSub_Id" },
            //    unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DatasetInstanceSubValue_ContextValue_DatasetInstanceValue_Id_DatasetElementSub_Id",
                table: "DatasetInstanceSubValue");

            migrationBuilder.CreateIndex(
                name: "IX_DatasetInstanceSubValue_DatasetInstanceValue_Id_DatasetElementSub_Id",
                table: "DatasetInstanceSubValue",
                columns: new[] { "DatasetInstanceValue_Id", "DatasetElementSub_Id" });
        }
    }
}
