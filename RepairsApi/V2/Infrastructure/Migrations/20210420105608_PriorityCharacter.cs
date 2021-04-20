using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class PriorityCharacter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<char>(
                name: "priority_character",
                table: "sor_priorities",
                type: "character(1)",
                nullable: false,
                defaultValue: 'E');

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_work_priority_priority_code",
                table: "work_orders",
                column: "work_priority_priority_code");

            migrationBuilder.Sql(@"
UPDATE work_orders
SET work_priority_priority_code =
    CASE
        WHEN work_priority_number_of_days = 0 THEN 1
        WHEN work_priority_number_of_days = 1 THEN 2
        WHEN work_priority_number_of_days = 7 THEN 3
        WHEN work_priority_number_of_days = 30 THEN 4
    END;

UPDATE sor_priorities
SET priority_character =
    CASE
        WHEN priority_code = 1 THEN 'I'
        WHEN priority_code = 2 THEN 'E'
        WHEN priority_code = 3 THEN 'U'
        WHEN priority_code = 4 THEN 'N'
    END;
");

            migrationBuilder.AddForeignKey(
                name: "fk_work_orders_sor_priorities_work_priority_priority_code1",
                table: "work_orders",
                column: "work_priority_priority_code",
                principalTable: "sor_priorities",
                principalColumn: "priority_code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_work_orders_sor_priorities_work_priority_priority_code1",
                table: "work_orders");

            migrationBuilder.DropIndex(
                name: "ix_work_orders_work_priority_priority_code",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "priority_character",
                table: "sor_priorities");
        }
    }
}
