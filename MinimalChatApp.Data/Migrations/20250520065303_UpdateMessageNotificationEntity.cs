using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalChatApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessageNotificationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MessageNotifications_MessageId",
                table: "MessageNotifications",
                column: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageNotifications_Messages_MessageId",
                table: "MessageNotifications",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "MessageId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageNotifications_Messages_MessageId",
                table: "MessageNotifications");

            migrationBuilder.DropIndex(
                name: "IX_MessageNotifications_MessageId",
                table: "MessageNotifications");
        }
    }
}
