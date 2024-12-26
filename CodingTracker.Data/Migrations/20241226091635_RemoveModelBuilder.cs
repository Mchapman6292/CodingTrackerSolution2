using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodingTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveModelBuilder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CodingSessions_UserCredentials_UserCredentialEntityUserId",
                table: "CodingSessions");

            migrationBuilder.DropIndex(
                name: "IX_UserCredentials_Username",
                table: "UserCredentials");

            migrationBuilder.DropIndex(
                name: "IX_CodingSessions_UserCredentialEntityUserId",
                table: "CodingSessions");

            migrationBuilder.DropColumn(
                name: "UserCredentialEntityUserId",
                table: "CodingSessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserCredentialEntityUserId",
                table: "CodingSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCredentials_Username",
                table: "UserCredentials",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CodingSessions_UserCredentialEntityUserId",
                table: "CodingSessions",
                column: "UserCredentialEntityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CodingSessions_UserCredentials_UserCredentialEntityUserId",
                table: "CodingSessions",
                column: "UserCredentialEntityUserId",
                principalTable: "UserCredentials",
                principalColumn: "UserId");
        }
    }
}
