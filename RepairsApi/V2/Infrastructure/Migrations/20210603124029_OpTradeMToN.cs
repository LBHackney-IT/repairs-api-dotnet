using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class OpTradeMToN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "ix_work_elements_operative_id",
                table: "work_elements");

            migrationBuilder.DropIndex(
                name: "ix_trade_operative_id",
                table: "trade");

            migrationBuilder.DropIndex(
                name: "ix_operatives_person_id",
                table: "operatives");

            migrationBuilder.DropIndex(
                name: "ix_operatives_work_order_complete_id",
                table: "operatives");

            migrationBuilder.DropColumn(
                name: "operative_id",
                table: "work_elements");

            migrationBuilder.DropColumn(
                name: "operative_id",
                table: "trade");

            migrationBuilder.DropColumn(
                name: "person_id",
                table: "operatives");

            migrationBuilder.DropColumn(
                name: "work_order_complete_id",
                table: "operatives");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "operatives",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "operative_sor_code_trade",
                columns: table => new
                {
                    operatives_id = table.Column<int>(type: "integer", nullable: false),
                    trades_code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_operative_sor_code_trade", x => new { x.operatives_id, x.trades_code });
                    table.ForeignKey(
                        name: "fk_operative_sor_code_trade_operatives_operatives_id",
                        column: x => x.operatives_id,
                        principalTable: "operatives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_operative_sor_code_trade_trades_trades_code",
                        column: x => x.trades_code,
                        principalTable: "trades",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_operative_sor_code_trade_trades_code",
                table: "operative_sor_code_trade",
                column: "trades_code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "operative_sor_code_trade");

            migrationBuilder.DropColumn(
                name: "name",
                table: "operatives");

            migrationBuilder.AddColumn<int>(
                name: "operative_id",
                table: "work_elements",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "operative_id",
                table: "trade",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "person_id",
                table: "operatives",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "work_order_complete_id",
                table: "operatives",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_work_elements_operative_id",
                table: "work_elements",
                column: "operative_id");

            migrationBuilder.CreateIndex(
                name: "ix_trade_operative_id",
                table: "trade",
                column: "operative_id");

            migrationBuilder.CreateIndex(
                name: "ix_operatives_person_id",
                table: "operatives",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_operatives_work_order_complete_id",
                table: "operatives",
                column: "work_order_complete_id");

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
    }
}
