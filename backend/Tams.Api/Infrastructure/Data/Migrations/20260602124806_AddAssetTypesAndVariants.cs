using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tams.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetTypesAndVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "asset_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    managed_by_department_id = table.Column<int>(type: "integer", nullable: false),
                    identifier_policy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    can_be_assigned_to_course = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_returnable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    show_in_calendar = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset_types", x => x.id);
                    table.ForeignKey(
                        name: "FK_asset_types_departments_managed_by_department_id",
                        column: x => x.managed_by_department_id,
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "asset_variants",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asset_type_id = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asset_variants", x => x.id);
                    table.ForeignKey(
                        name: "FK_asset_variants_asset_types_asset_type_id",
                        column: x => x.asset_type_id,
                        principalTable: "asset_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "asset_types",
                columns: new[] { "id", "can_be_assigned_to_course", "code", "color", "created_at_utc", "description", "icon", "identifier_policy", "is_active", "is_returnable", "managed_by_department_id", "name", "show_in_calendar", "sort_order", "updated_at_utc" },
                values: new object[,]
                {
                    { 1, true, "CARD", "#0d6efd", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "NFC card managed by TCO.", "credit-card", "Rfid", true, true, 1, "Card", true, 10, null },
                    { 2, true, "LAPTOP", "#198754", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop managed by ITC Support.", "laptop", "Barcode", true, true, 2, "Laptop", true, 20, null },
                    { 3, true, "USB", "#6f42c1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "USB device managed by ITC Support.", "usb", "Barcode", true, true, 2, "USB", true, 30, null }
                });

            migrationBuilder.InsertData(
                table: "asset_variants",
                columns: new[] { "id", "asset_type_id", "code", "created_at_utc", "description", "is_active", "name", "sort_order", "updated_at_utc" },
                values: new object[,]
                {
                    { 1, 1, "VENDING", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cards physically assigned to vending use.", true, "Vending", 10, null },
                    { 2, 1, "CANTEEN", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cards physically assigned to canteen use.", true, "Canteen", 20, null },
                    { 3, 1, "INSTRUCTOR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cards physically assigned to instructors.", true, "Instructor", 30, null },
                    { 4, 1, "GUEST", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cards physically assigned to guests.", true, "Guest", 40, null },
                    { 5, 2, "G8", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop model G8.", true, "G8", 10, null },
                    { 6, 2, "G10", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop model G10.", true, "G10", 20, null },
                    { 7, 2, "G11", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop model G11.", true, "G11", 30, null },
                    { 8, 3, "STANDARD", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Standard USB device.", true, "Standard", 10, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_asset_types_code",
                table: "asset_types",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_asset_types_managed_by_department_id",
                table: "asset_types",
                column: "managed_by_department_id");

            migrationBuilder.CreateIndex(
                name: "IX_asset_variants_asset_type_id_code",
                table: "asset_variants",
                columns: new[] { "asset_type_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asset_variants");

            migrationBuilder.DropTable(
                name: "asset_types");
        }
    }
}
