using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class Appointments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "enabled",
                table: "sor_codes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "available_appointments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    contractor_reference = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_available_appointments", x => x.id);
                    table.ForeignKey(
                        name: "fk_available_appointments_contractors_contractor_reference",
                        column: x => x.contractor_reference,
                        principalTable: "contractors",
                        principalColumn: "reference",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "available_appointment_days",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    available_appointment_id = table.Column<int>(type: "integer", nullable: false),
                    day = table.Column<int>(type: "integer", nullable: false),
                    available_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_available_appointment_days", x => x.id);
                    table.ForeignKey(
                        name: "fk_available_appointment_days_available_appointments_available",
                        column: x => x.available_appointment_id,
                        principalTable: "available_appointments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    day_id = table.Column<int>(type: "integer", nullable: false),
                    work_order_id = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointments", x => new { x.day_id, x.work_order_id });
                    table.ForeignKey(
                        name: "fk_appointments_available_appointment_days_day_id",
                        column: x => x.day_id,
                        principalTable: "available_appointment_days",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_appointments_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_appointments_work_order_id",
                table: "appointments",
                column: "work_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_available_appointment_days_available_appointment_id",
                table: "available_appointment_days",
                column: "available_appointment_id");

            migrationBuilder.CreateIndex(
                name: "ix_available_appointments_contractor_reference",
                table: "available_appointments",
                column: "contractor_reference");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "available_appointment_days");

            migrationBuilder.DropTable(
                name: "available_appointments");

            migrationBuilder.DropColumn(
                name: "enabled",
                table: "sor_codes");
        }
    }
}
