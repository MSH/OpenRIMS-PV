using Microsoft.EntityFrameworkCore.Migrations;

namespace PViMS.Infrastructure.Migrations
{
    public partial class ModifyConceptAndProductIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_Concept_ConceptName_MedicationForm_Id",
            //    table: "Concept");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Product_Concept_Id_ProductName_Manufacturer",
            //    table: "Product",
            //    columns: new[] { "Concept_Id", "ProductName", "Manufacturer" },
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Concept_ConceptName_Strength_MedicationForm_Id",
            //    table: "Concept",
            //    columns: new[] { "ConceptName", "Strength", "MedicationForm_Id" },
            //    unique: true,
            //    filter: "[Strength] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_Concept_Id_ProductName_Manufacturer",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Concept_ConceptName_Strength_MedicationForm_Id",
                table: "Concept");

            migrationBuilder.CreateIndex(
                name: "IX_Concept_ConceptName_MedicationForm_Id",
                table: "Concept",
                columns: new[] { "ConceptName", "MedicationForm_Id" },
                unique: true);
        }
    }
}
