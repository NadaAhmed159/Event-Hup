using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserStatusAddOrganizerApproved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EventOrganizerApproved",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("""
                UPDATE [Users] SET [EventOrganizerApproved] = CAST(1 AS bit)
                WHERE [Status] = N'Approved' OR [ApplyAs] <> N'EventOrganizer';

                UPDATE [Users] SET [EventOrganizerApproved] = CAST(0 AS bit)
                WHERE [ApplyAs] = N'EventOrganizer' AND [Status] = N'Pending';

                UPDATE [Users] SET [EventOrganizerApproved] = CAST(1 AS bit), [ApplyAs] = N'Participant'
                WHERE [ApplyAs] = N'EventOrganizer' AND [Status] = N'Rejected';
                """);

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE [Users] SET [Status] = CASE
                    WHEN [ApplyAs] = N'EventOrganizer' AND [EventOrganizerApproved] = CAST(0 AS bit) THEN N'Pending'
                    ELSE N'Approved'
                END;
                """);

            migrationBuilder.DropColumn(
                name: "EventOrganizerApproved",
                table: "Users");
        }
    }
}
