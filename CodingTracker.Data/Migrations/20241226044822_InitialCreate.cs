using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CodingTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCredentials",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredentials", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "CodingSessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DurationSeconds = table.Column<double>(type: "double precision", nullable: true),
                    DurationHHMM = table.Column<string>(type: "text", nullable: true),
                    GoalHHMM = table.Column<string>(type: "text", nullable: true),
                    GoalReached = table.Column<int>(type: "integer", nullable: true),
                    UserCredentialEntityUserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodingSessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_CodingSessions_UserCredentials_UserCredentialEntityUserId",
                        column: x => x.UserCredentialEntityUserId,
                        principalTable: "UserCredentials",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodingSessions_UserCredentialEntityUserId",
                table: "CodingSessions",
                column: "UserCredentialEntityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredentials_Username",
                table: "UserCredentials",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodingSessions");

            migrationBuilder.DropTable(
                name: "UserCredentials");
        }
    }
}
