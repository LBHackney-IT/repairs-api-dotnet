using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V1.Infrastructure.Migrations
{
    public partial class AddWorkElement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "work_element_id",
                table: "rate_schedule_item",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "work_element",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    trade = table.Column<string>(name: "trade ", nullable: true),
                    service_charge_subject = table.Column<string>(nullable: true),
                    contains_capital_work = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_element", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rate_schedule_item_work_element_id",
                table: "rate_schedule_item",
                column: "work_element_id");

            migrationBuilder.AddForeignKey(
                name: "FK_rate_schedule_item_work_element_work_element_id",
                table: "rate_schedule_item",
                column: "work_element_id",
                principalTable: "work_element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rate_schedule_item_work_element_work_element_id",
                table: "rate_schedule_item");

            migrationBuilder.DropTable(
                name: "work_element");

            migrationBuilder.DropIndex(
                name: "IX_rate_schedule_item_work_element_id",
                table: "rate_schedule_item");

            migrationBuilder.DropColumn(
                name: "work_element_id",
                table: "rate_schedule_item");
        }
    }
}
