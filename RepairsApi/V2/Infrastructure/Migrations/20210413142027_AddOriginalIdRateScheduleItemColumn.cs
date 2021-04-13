using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class AddOriginalIdRateScheduleItemColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "original_id",
                table: "rate_schedule_item",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "original_id",
                table: "rate_schedule_item");
        }
    }
}
