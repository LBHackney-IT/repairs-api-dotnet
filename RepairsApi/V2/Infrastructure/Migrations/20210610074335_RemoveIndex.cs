using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class RemoveIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "work_order_operatives",
                columns: table => new
                {
                    work_order_id = table.Column<int>(type: "integer", nullable: false),
                    operative_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_order_operatives", x => new { x.work_order_id, x.operative_id });
                    table.ForeignKey(
                        name: "fk_work_order_operatives_operatives_operative_id",
                        column: x => x.operative_id,
                        principalTable: "operatives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_work_order_operatives_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_work_order_operatives_operative_id",
                table: "work_order_operatives",
                column: "operative_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "work_order_operatives");
        }
    }
}
