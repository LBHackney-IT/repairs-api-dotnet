using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class AppointmentKeyAndStartEndTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_appointments_available_appointment_days_day_id",
                table: "appointments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_appointments",
                table: "appointments");

            migrationBuilder.AlterColumn<int>(
                name: "day_id",
                table: "appointments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "end_time",
                table: "appointments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "start_time",
                table: "appointments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "pk_appointments",
                table: "appointments",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_appointments_day_id",
                table: "appointments",
                column: "day_id");

            migrationBuilder.AddForeignKey(
                name: "fk_appointments_available_appointment_days_day_id",
                table: "appointments",
                column: "day_id",
                principalTable: "available_appointment_days",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(
                @"UPDATE appointments app SET
                start_time = aa.start_time,
                end_time = aa.end_time
                FROM available_appointment_days aad
                INNER JOIN available_appointments aa ON aa.id = aad.available_appointment_id
                WHERE aad.id = app.day_id"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_appointments_available_appointment_days_day_id",
                table: "appointments");

            migrationBuilder.DropPrimaryKey(
                name: "pk_appointments",
                table: "appointments");

            migrationBuilder.DropIndex(
                name: "ix_appointments_day_id",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "id",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "end_time",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "start_time",
                table: "appointments");

            migrationBuilder.AlterColumn<int>(
                name: "day_id",
                table: "appointments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_appointments",
                table: "appointments",
                columns: new[] { "day_id", "work_order_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_appointments_available_appointment_days_day_id",
                table: "appointments",
                column: "day_id",
                principalTable: "available_appointment_days",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
