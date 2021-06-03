using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class OpTradeMToN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_trade_operatives_operative_id",
                table: "trade");

            migrationBuilder.DropIndex(
                name: "ix_trade_operative_id",
                table: "trade");

            migrationBuilder.DropColumn(
                name: "operative_id",
                table: "trade");

            migrationBuilder.CreateTable(
                name: "operative_trade",
                columns: table => new
                {
                    operatives_id = table.Column<int>(type: "integer", nullable: false),
                    trade_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_operative_trade", x => new { x.operatives_id, x.trade_id });
                    table.ForeignKey(
                        name: "fk_operative_trade_operatives_operatives_id",
                        column: x => x.operatives_id,
                        principalTable: "operatives",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_operative_trade_trade_trade_id",
                        column: x => x.trade_id,
                        principalTable: "trade",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_operative_trade_trade_id",
                table: "operative_trade",
                column: "trade_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "operative_trade");

            migrationBuilder.AddColumn<int>(
                name: "operative_id",
                table: "trade",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_trade_operative_id",
                table: "trade",
                column: "operative_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trade_operatives_operative_id",
                table: "trade",
                column: "operative_id",
                principalTable: "operatives",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
