using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class WorkOrderCompleteOneToOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_work_order_completes_work_orders_work_order_id",
                table: "work_order_completes");

            migrationBuilder.DropForeignKey(
                name: "fk_work_orders_work_order_completes_work_order_complete_id",
                table: "work_orders");

            migrationBuilder.DropIndex(
                name: "ix_work_order_completes_work_order_id",
                table: "work_order_completes");

            migrationBuilder.DropColumn(
                name: "work_order_id",
                table: "work_order_completes");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "work_order_completes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "fk_work_order_completes_work_orders_id",
                table: "work_order_completes",
                column: "id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_work_orders_work_order_completes_work_order_complete_id1",
                table: "work_orders",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_work_order_completes_work_orders_id",
                table: "work_order_completes");

            migrationBuilder.DropForeignKey(
                name: "fk_work_orders_work_order_completes_work_order_complete_id1",
                table: "work_orders");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "work_order_completes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "work_order_id",
                table: "work_order_completes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_work_order_completes_work_order_id",
                table: "work_order_completes",
                column: "work_order_id");

            migrationBuilder.AddForeignKey(
                name: "fk_work_order_completes_work_orders_work_order_id",
                table: "work_order_completes",
                column: "work_order_id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_orders_work_order_completes_work_order_complete_id",
                table: "work_orders",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
