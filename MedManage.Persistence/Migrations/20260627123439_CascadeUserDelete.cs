using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedManage.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CascadeUserDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Users_CreatedByUserId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_InAppNotifications_Users_RecipientUserId",
                table: "InAppNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_InAppNotifications_Users_SenderUserId",
                table: "InAppNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationOutbox_Users_RecipientUserId",
                table: "NotificationOutbox");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Announcements_AnnouncementId",
                table: "PurchaseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Users_BuyerUserId",
                table: "PurchaseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Users_SellerUserId",
                table: "PurchaseRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Users_CreatedByUserId",
                table: "Announcements",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InAppNotifications_Users_RecipientUserId",
                table: "InAppNotifications",
                column: "RecipientUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InAppNotifications_Users_SenderUserId",
                table: "InAppNotifications",
                column: "SenderUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationOutbox_Users_RecipientUserId",
                table: "NotificationOutbox",
                column: "RecipientUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Announcements_AnnouncementId",
                table: "PurchaseRequests",
                column: "AnnouncementId",
                principalTable: "Announcements",
                principalColumn: "AnnouncementId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Users_BuyerUserId",
                table: "PurchaseRequests",
                column: "BuyerUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Users_SellerUserId",
                table: "PurchaseRequests",
                column: "SellerUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Users_CreatedByUserId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_InAppNotifications_Users_RecipientUserId",
                table: "InAppNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_InAppNotifications_Users_SenderUserId",
                table: "InAppNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationOutbox_Users_RecipientUserId",
                table: "NotificationOutbox");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Announcements_AnnouncementId",
                table: "PurchaseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Users_BuyerUserId",
                table: "PurchaseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Users_SellerUserId",
                table: "PurchaseRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Users_CreatedByUserId",
                table: "Announcements",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InAppNotifications_Users_RecipientUserId",
                table: "InAppNotifications",
                column: "RecipientUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InAppNotifications_Users_SenderUserId",
                table: "InAppNotifications",
                column: "SenderUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationOutbox_Users_RecipientUserId",
                table: "NotificationOutbox",
                column: "RecipientUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Announcements_AnnouncementId",
                table: "PurchaseRequests",
                column: "AnnouncementId",
                principalTable: "Announcements",
                principalColumn: "AnnouncementId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Users_BuyerUserId",
                table: "PurchaseRequests",
                column: "BuyerUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Users_SellerUserId",
                table: "PurchaseRequests",
                column: "SellerUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
