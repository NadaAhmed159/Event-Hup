using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAccountStatusReplaceOrganizerFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Approved");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EventOrganizerApproved",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.Sql("""
                UPDATE [Users] SET [EventOrganizerApproved] = CASE
                    WHEN [ApplyAs] = N'EventOrganizer' AND [Status] = N'Pending' THEN CAST(0 AS bit)
                    ELSE CAST(1 AS bit)
                END;
                """);

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");
        }
    }
}
