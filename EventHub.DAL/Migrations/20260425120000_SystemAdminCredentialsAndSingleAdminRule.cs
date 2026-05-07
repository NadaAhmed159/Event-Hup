using EventHub.Domain;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHub.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SystemAdminCredentialsAndSingleAdminRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
                UPDATE [Users] SET [ApplyAs] = N'Participant', [Status] = N'Approved'
                WHERE [ApplyAs] = N'Admin' AND [Id] <> N'{SystemAdmin.UserId}';

                UPDATE [Users] SET [Password] = N'{SystemAdmin.SeededPasswordHash}',
                    [Email] = N'{SystemAdmin.Email}',
                    [ApplyAs] = N'Admin',
                    [Status] = N'Approved',
                    [FirstName] = N'Admin',
                    [LastName] = N'User'
                WHERE [Id] = N'{SystemAdmin.UserId}';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""
                UPDATE [Users] SET [Password] = N'123456' WHERE [Id] = N'{SystemAdmin.UserId}';
                """);
        }
    }
}
