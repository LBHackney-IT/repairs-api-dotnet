﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class AddStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status_code",
                table: "work_orders",
                type: "integer",
                nullable: false,
                defaultValue: 80);

            migrationBuilder.Sql("UPDATE public.work_orders SET status_code=50 WHERE work_order_complete_id IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status_code",
                table: "work_orders");
        }
    }
}
