using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class AddBooleanVariationsColumnToWorkorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "has_variation",
                table: "work_orders",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "has_variation",
                table: "work_orders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }
    }
}
