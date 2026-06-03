using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tams.Api.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    asset_variant_id = table.Column<int>(type: "integer", nullable: false),
                    uid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    serial_number = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Available"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_devices", x => x.id);
                    table.ForeignKey(
                        name: "FK_devices_asset_variants_asset_variant_id",
                        column: x => x.asset_variant_id,
                        principalTable: "asset_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_devices_asset_variant_id",
                table: "devices",
                column: "asset_variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_devices_barcode",
                table: "devices",
                column: "barcode",
                unique: true,
                filter: "barcode IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_devices_name",
                table: "devices",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_devices_serial_number",
                table: "devices",
                column: "serial_number",
                unique: true,
                filter: "serial_number IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_devices_status",
                table: "devices",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_devices_uid",
                table: "devices",
                column: "uid",
                unique: true,
                filter: "uid IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "devices");
        }
    }
}
