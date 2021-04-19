using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class AddStandardMinuteValueColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "standard_minute_value",
                table: "sor_codes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "standard_minute_value",
                table: "sor_codes");
        }
    }
}
