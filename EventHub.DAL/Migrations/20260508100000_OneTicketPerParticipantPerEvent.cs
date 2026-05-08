using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.DAL.Migrations
{
    /// <inheritdoc />
    public partial class OneTicketPerParticipantPerEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Older multi-quantity bookings left multiple Ticket rows per (ParticipantId, EventId).
            // Keep one row per pair before adding the composite unique index.
            migrationBuilder.Sql(
                """
                DELETE FROM [Tickets]
                WHERE [Id] IN (
                    SELECT [Id]
                    FROM (
                        SELECT [Id],
                            ROW_NUMBER() OVER (
                                PARTITION BY [ParticipantId], [EventId]
                                ORDER BY [PurchasedAt], [Id]) AS [rn]
                        FROM [Tickets]
                    ) AS [d]
                    WHERE [d].[rn] > 1
                );
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ParticipantId_EventId",
                table: "Tickets",
                columns: new[] { "ParticipantId", "EventId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_ParticipantId_EventId",
                table: "Tickets");
        }
    }
}
