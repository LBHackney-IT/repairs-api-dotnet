using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V1.Infrastructure.Migrations
{
    public partial class SplitQuantityFromRateScheduleItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quantity",
                table: "rate_schedule_item");

            migrationBuilder.DropColumn(
                name: "unit_of_measurement",
                table: "rate_schedule_item");

            migrationBuilder.AddColumn<int>(
                name: "amount",
                table: "rate_schedule_item",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "unit_of_measurement_code",
                table: "rate_schedule_item",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "amount",
                table: "rate_schedule_item");

            migrationBuilder.DropColumn(
                name: "unit_of_measurement_code",
                table: "rate_schedule_item");

            migrationBuilder.AddColumn<int>(
                name: "quantity",
                table: "rate_schedule_item",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "unit_of_measurement",
                table: "rate_schedule_item",
                type: "text",
                nullable: true);
        }
    }
}
