using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tams.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedDepartments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "departments",
                columns: new[] { "id", "code", "created_at_utc", "is_active", "name", "updated_at_utc" },
                values: new object[,]
                {
                    { 1, "TCO", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "TCO", null },
                    { 2, "ITC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "ITC Support", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "departments",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "departments",
                keyColumn: "id",
                keyValue: 2);
        }
    }
}
