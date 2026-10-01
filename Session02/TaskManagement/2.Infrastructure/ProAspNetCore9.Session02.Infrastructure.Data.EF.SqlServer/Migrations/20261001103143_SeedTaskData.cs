using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProAspNetCore9.Session02.Infrastructure.Data.EF.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class SeedTaskData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TaskItems",
                columns: new[] { "Id", "CreatedAt", "Description", "DueDate", "IsDeleted", "Status", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 28, 10, 0, 0, 0, DateTimeKind.Unspecified), "Practice manual routing and HttpContext", new DateTime(2026, 10, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0, "Learn ASP.NET Core" },
                    { 2, new DateTime(2026, 9, 29, 11, 0, 0, 0, DateTimeKind.Unspecified), "Practice DbContext and repository", new DateTime(2026, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 1, "Learn EF Core" },
                    { 3, new DateTime(2026, 10, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Test endpoint using Postman", new DateTime(2026, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 0, "Test GET endpoint" },
                    { 4, new DateTime(2026, 10, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Review architecture and dependencies", new DateTime(2026, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), false, 2, "Refactor Task Management" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
