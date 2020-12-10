using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V1.Infrastructure.Migrations
{
    public partial class AddRateScheduleItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rate_schedule_item",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    custom_code = table.Column<string>(nullable: true),
                    custom_name = table.Column<string>(nullable: true),
                    quantity = table.Column<int>(nullable: false),
                    unit_of_measurement = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rate_schedule_item", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rate_schedule_item");
        }
    }
}
