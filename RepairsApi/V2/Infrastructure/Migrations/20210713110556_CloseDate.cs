using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class CloseDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "closed_date",
                table: "work_orders",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE public.work_orders wo SET closed_date = jsu.event_time
FROM public.job_status_updates jsu 
WHERE wo.id = jsu.work_order_complete_id
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "closed_date",
                table: "work_orders");
        }
    }
}
