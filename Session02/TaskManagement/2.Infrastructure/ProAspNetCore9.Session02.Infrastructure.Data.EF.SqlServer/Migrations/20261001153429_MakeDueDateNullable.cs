using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProAspNetCore9.Session02.Infrastructure.Data.EF.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class MakeDueDateNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "TaskItems",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.InsertData(
                table: "TaskItems",
                columns: new[] { "Id", "CreatedAt", "Description", "DueDate", "IsDeleted", "Status", "Title" },
                values: new object[] { 5, new DateTime(2026, 10, 1, 14, 0, 0, 0, DateTimeKind.Unspecified), "This task does not have a due date", null, false, 0, "Task without due date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "TaskItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
