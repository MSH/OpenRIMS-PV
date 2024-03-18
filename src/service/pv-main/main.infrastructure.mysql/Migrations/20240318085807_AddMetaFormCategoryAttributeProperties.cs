using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenRIMS.PV.Main.Infrastructure.MySQL.Migrations
{
    /// <inheritdoc />
    public partial class AddMetaFormCategoryAttributeProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<int>(
                name: "FormAttributeTypeId",
                table: "MetaFormCategoryAttribute",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "FutureDateOnly",
                table: "MetaFormCategoryAttribute",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "MetaFormCategoryAttribute",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumericMaxValue",
                table: "MetaFormCategoryAttribute",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumericMinValue",
                table: "MetaFormCategoryAttribute",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PastDateOnly",
                table: "MetaFormCategoryAttribute",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SelectionDataItem",
                table: "MetaFormCategoryAttribute",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "StringMaxLength",
                table: "MetaFormCategoryAttribute",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormAttributeTypeId",
                table: "MetaFormCategoryAttribute");

            migrationBuilder.DropColumn(
                name: "FutureDateOnly",
                table: "MetaFormCategoryAttribute");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "MetaFormCategoryAttribute");

            migrationBuilder.DropColumn(
                name: "NumericMaxValue",
                table: "MetaFormCategoryAttribute");

            migrationBuilder.DropColumn(
                name: "NumericMinValue",
                table: "MetaFormCategoryAttribute");

            migrationBuilder.DropColumn(
                name: "PastDateOnly",
                table: "MetaFormCategoryAttribute");

            migrationBuilder.DropColumn(
                name: "SelectionDataItem",
                table: "MetaFormCategoryAttribute");

            migrationBuilder.DropColumn(
                name: "StringMaxLength",
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
        }
    }
}
