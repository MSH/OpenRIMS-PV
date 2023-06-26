using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PViMS.Infrastructure.Migrations
{
    public partial class AddNotificationAggregate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DestinationUser_Id = table.Column<int>(type: "int", nullable: false),
                    ValidUntilDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy_Id = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy_Id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_User_CreatedBy_Id",
                        column: x => x.CreatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notification_User_DestinationUser_Id",
                        column: x => x.DestinationUser_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Notification_User_UpdatedBy_Id",
                        column: x => x.UpdatedBy_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_CreatedBy_Id",
                table: "Notification",
                column: "CreatedBy_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_DestinationUser_Id",
                table: "Notification",
                column: "DestinationUser_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UpdatedBy_Id",
                table: "Notification",
                column: "UpdatedBy_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");
        }
    }
}
