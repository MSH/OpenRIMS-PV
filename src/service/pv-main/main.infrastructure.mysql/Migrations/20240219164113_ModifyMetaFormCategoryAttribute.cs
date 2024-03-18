using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenRIMS.PV.Main.Infrastructure.MySQL.Migrations
{
    /// <inheritdoc />
    public partial class ModifyMetaFormCategoryAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "metaformcategory_guid",
                table: "MetaFormCategoryAttribute",
                newName: "metaformcategoryattribute_guid");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReportInstanceMedicationGuid",
                table: "ReportInstanceMedication",
                type: "char(30)",
                maxLength: 30,
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "char(30)",
                oldMaxLength: 30)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AttributeName",
                table: "MetaFormCategoryAttribute",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttributeName",
                table: "MetaFormCategoryAttribute");

            migrationBuilder.RenameColumn(
                name: "metaformcategoryattribute_guid",
                table: "MetaFormCategoryAttribute",
                newName: "metaformcategory_guid");

            migrationBuilder.AlterColumn<string>(
                name: "ReportInstanceMedicationGuid",
                table: "ReportInstanceMedication",
                type: "char(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(30)",
                oldMaxLength: 30)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
