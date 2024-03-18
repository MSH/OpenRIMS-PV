using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenRIMS.PV.Main.Infrastructure.MySQL.Migrations
{
    /// <inheritdoc />
    public partial class CustomAttributeOnMetaCategoryAttributeNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetaFormCategoryAttribute_CustomAttributeConfiguration_Confi~",
                table: "MetaFormCategoryAttribute");

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

            migrationBuilder.AlterColumn<int>(
                name: "Configuration_Id",
                table: "MetaFormCategoryAttribute",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_MetaFormCategoryAttribute_CustomAttributeConfiguration_Confi~",
                table: "MetaFormCategoryAttribute",
                column: "Configuration_Id",
                principalTable: "CustomAttributeConfiguration",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetaFormCategoryAttribute_CustomAttributeConfiguration_Confi~",
                table: "MetaFormCategoryAttribute");

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

            migrationBuilder.AlterColumn<int>(
                name: "Configuration_Id",
                table: "MetaFormCategoryAttribute",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MetaFormCategoryAttribute_CustomAttributeConfiguration_Confi~",
                table: "MetaFormCategoryAttribute",
                column: "Configuration_Id",
                principalTable: "CustomAttributeConfiguration",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
