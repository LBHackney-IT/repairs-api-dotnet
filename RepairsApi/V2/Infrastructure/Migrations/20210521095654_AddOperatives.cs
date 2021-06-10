using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class AddOperatives : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_operative_person_person_id",
                table: "operative");

            migrationBuilder.DropForeignKey(
                name: "fk_operative_work_order_completes_work_order_complete_id",
                table: "operative");

            migrationBuilder.DropForeignKey(
                name: "fk_trade_operative_operative_id",
                table: "trade");

            migrationBuilder.DropForeignKey(
                name: "fk_work_elements_operative_operative_id",
                table: "work_elements");

            migrationBuilder.DropPrimaryKey(
                name: "pk_operative",
                table: "operative");

            migrationBuilder.RenameTable(
                name: "operative",
                newName: "operatives");

            migrationBuilder.RenameIndex(
                name: "ix_operative_work_order_complete_id",
                table: "operatives",
                newName: "ix_operatives_work_order_complete_id");

            migrationBuilder.RenameIndex(
                name: "ix_operative_person_id",
                table: "operatives",
                newName: "ix_operatives_person_id");

            migrationBuilder.AddColumn<string>(
                name: "payroll_number",
                table: "operatives",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_operatives",
                table: "operatives",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_operatives_person_person_id",
                table: "operatives",
                column: "person_id",
                principalTable: "person",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_operatives_work_order_completes_work_order_complete_id",
                table: "operatives",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_trade_operatives_operative_id",
                table: "trade",
                column: "operative_id",
                principalTable: "operatives",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_elements_operatives_operative_id",
                table: "work_elements",
                column: "operative_id",
                principalTable: "operatives",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_operatives_person_person_id",
                table: "operatives");

            migrationBuilder.DropForeignKey(
                name: "fk_operatives_work_order_completes_work_order_complete_id",
                table: "operatives");

            migrationBuilder.DropForeignKey(
                name: "fk_trade_operatives_operative_id",
                table: "trade");

            migrationBuilder.DropForeignKey(
                name: "fk_work_elements_operatives_operative_id",
                table: "work_elements");

            migrationBuilder.DropPrimaryKey(
                name: "pk_operatives",
                table: "operatives");

            migrationBuilder.DropColumn(
                name: "payroll_number",
                table: "operatives");

            migrationBuilder.RenameTable(
                name: "operatives",
                newName: "operative");

            migrationBuilder.RenameIndex(
                name: "ix_operatives_work_order_complete_id",
                table: "operative",
                newName: "ix_operative_work_order_complete_id");

            migrationBuilder.RenameIndex(
                name: "ix_operatives_person_id",
                table: "operative",
                newName: "ix_operative_person_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_operative",
                table: "operative",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_operative_person_person_id",
                table: "operative",
                column: "person_id",
                principalTable: "person",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_operative_work_order_completes_work_order_complete_id",
                table: "operative",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_trade_operative_operative_id",
                table: "trade",
                column: "operative_id",
                principalTable: "operative",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_elements_operative_operative_id",
                table: "work_elements",
                column: "operative_id",
                principalTable: "operative",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
