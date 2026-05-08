using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.DAL.Migrations
{
    /// <inheritdoc />
    public partial class testdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_ParticipantId",
                table: "Tickets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ParticipantId",
                table: "Tickets",
                column: "ParticipantId");
        }
    }
}
