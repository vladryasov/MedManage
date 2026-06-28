using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedManage.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixAnnouncementFkNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Announcements_AnnouncementId",
                table: "PurchaseRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "AnnouncementId",
                table: "PurchaseRequests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Announcements_AnnouncementId",
                table: "PurchaseRequests",
                column: "AnnouncementId",
                principalTable: "Announcements",
                principalColumn: "AnnouncementId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Announcements_AnnouncementId",
                table: "PurchaseRequests");

            migrationBuilder.AlterColumn<Guid>(
                name: "AnnouncementId",
                table: "PurchaseRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Announcements_AnnouncementId",
                table: "PurchaseRequests",
                column: "AnnouncementId",
                principalTable: "Announcements",
                principalColumn: "AnnouncementId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
