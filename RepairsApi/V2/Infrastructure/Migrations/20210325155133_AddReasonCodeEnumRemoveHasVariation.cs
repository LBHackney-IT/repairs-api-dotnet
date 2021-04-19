using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class AddReasonCodeEnumRemoveHasVariation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "has_variation",
                table: "work_orders");

            migrationBuilder.AddColumn<int>(
                name: "reason",
                table: "work_orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reason",
                table: "work_orders");

            migrationBuilder.AddColumn<bool>(
                name: "has_variation",
                table: "work_orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
