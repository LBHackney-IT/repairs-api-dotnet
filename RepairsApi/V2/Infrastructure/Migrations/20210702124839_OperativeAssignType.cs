using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class OperativeAssignType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "assignment_type",
                table: "work_order_operatives",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "assignment_type",
                table: "work_order_operatives");
        }
    }
}
