using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class CostColumnAndRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_job_status_updates_work_element_more_specific_sor_code_id",
                table: "job_status_updates");

            migrationBuilder.DropForeignKey(
                name: "fk_rate_schedule_item_work_element_work_element_id",
                table: "rate_schedule_item");

            migrationBuilder.DropForeignKey(
                name: "fk_trade_work_element_work_element_id",
                table: "trade");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_job_status_updates_job_status_update_id",
                table: "work_element");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_operative_operative_id",
                table: "work_element");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_work_order_completes_work_order_complete_id",
                table: "work_element");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_work_orders_work_order_id",
                table: "work_element");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_dependency_work_element_depends_on_work_elemen",
                table: "work_element_dependency");

            migrationBuilder.DropPrimaryKey(
                name: "pk_work_element",
                table: "work_element");

            migrationBuilder.RenameTable(
                name: "work_element",
                newName: "work_elements");

            migrationBuilder.RenameIndex(
                name: "ix_work_element_work_order_id",
                table: "work_elements",
                newName: "ix_work_elements_work_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_element_work_order_complete_id",
                table: "work_elements",
                newName: "ix_work_elements_work_order_complete_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_element_operative_id",
                table: "work_elements",
                newName: "ix_work_elements_operative_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_element_job_status_update_id",
                table: "work_elements",
                newName: "ix_work_elements_job_status_update_id");

            migrationBuilder.AddColumn<double>(
                name: "cost",
                table: "sor_codes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "code_cost",
                table: "rate_schedule_item",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                table: "rate_schedule_item",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_work_elements",
                table: "work_elements",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_updates_work_elements_more_specific_sor_code_id",
                table: "job_status_updates",
                column: "more_specific_sor_code_id",
                principalTable: "work_elements",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rate_schedule_item_work_elements_work_element_id",
                table: "rate_schedule_item",
                column: "work_element_id",
                principalTable: "work_elements",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_trade_work_elements_work_element_id",
                table: "trade",
                column: "work_element_id",
                principalTable: "work_elements",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_dependency_work_elements_depends_on_work_eleme",
                table: "work_element_dependency",
                column: "depends_on_work_element_id",
                principalTable: "work_elements",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_elements_job_status_updates_job_status_update_id",
                table: "work_elements",
                column: "job_status_update_id",
                principalTable: "job_status_updates",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_elements_operative_operative_id",
                table: "work_elements",
                column: "operative_id",
                principalTable: "operative",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_elements_work_order_completes_work_order_complete_id",
                table: "work_elements",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_elements_work_orders_work_order_id",
                table: "work_elements",
                column: "work_order_id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_job_status_updates_work_elements_more_specific_sor_code_id",
                table: "job_status_updates");

            migrationBuilder.DropForeignKey(
                name: "fk_rate_schedule_item_work_elements_work_element_id",
                table: "rate_schedule_item");

            migrationBuilder.DropForeignKey(
                name: "fk_trade_work_elements_work_element_id",
                table: "trade");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_dependency_work_elements_depends_on_work_eleme",
                table: "work_element_dependency");

            migrationBuilder.DropForeignKey(
                name: "fk_work_elements_job_status_updates_job_status_update_id",
                table: "work_elements");

            migrationBuilder.DropForeignKey(
                name: "fk_work_elements_operative_operative_id",
                table: "work_elements");

            migrationBuilder.DropForeignKey(
                name: "fk_work_elements_work_order_completes_work_order_complete_id",
                table: "work_elements");

            migrationBuilder.DropForeignKey(
                name: "fk_work_elements_work_orders_work_order_id",
                table: "work_elements");

            migrationBuilder.DropPrimaryKey(
                name: "pk_work_elements",
                table: "work_elements");

            migrationBuilder.DropColumn(
                name: "cost",
                table: "sor_codes");

            migrationBuilder.DropColumn(
                name: "code_cost",
                table: "rate_schedule_item");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "rate_schedule_item");

            migrationBuilder.RenameTable(
                name: "work_elements",
                newName: "work_element");

            migrationBuilder.RenameIndex(
                name: "ix_work_elements_work_order_id",
                table: "work_element",
                newName: "ix_work_element_work_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_elements_work_order_complete_id",
                table: "work_element",
                newName: "ix_work_element_work_order_complete_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_elements_operative_id",
                table: "work_element",
                newName: "ix_work_element_operative_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_elements_job_status_update_id",
                table: "work_element",
                newName: "ix_work_element_job_status_update_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_work_element",
                table: "work_element",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_updates_work_element_more_specific_sor_code_id",
                table: "job_status_updates",
                column: "more_specific_sor_code_id",
                principalTable: "work_element",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rate_schedule_item_work_element_work_element_id",
                table: "rate_schedule_item",
                column: "work_element_id",
                principalTable: "work_element",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_trade_work_element_work_element_id",
                table: "trade",
                column: "work_element_id",
                principalTable: "work_element",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_job_status_updates_job_status_update_id",
                table: "work_element",
                column: "job_status_update_id",
                principalTable: "job_status_updates",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_operative_operative_id",
                table: "work_element",
                column: "operative_id",
                principalTable: "operative",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_work_order_completes_work_order_complete_id",
                table: "work_element",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_work_orders_work_order_id",
                table: "work_element",
                column: "work_order_id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_dependency_work_element_depends_on_work_elemen",
                table: "work_element_dependency",
                column: "depends_on_work_element_id",
                principalTable: "work_element",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
