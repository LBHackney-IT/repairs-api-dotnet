using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class EnabledPriorities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "days_to_complete",
                table: "sor_priorities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "enabled",
                table: "sor_priorities",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "days_to_complete",
                table: "sor_priorities");

            migrationBuilder.DropColumn(
                name: "enabled",
                table: "sor_priorities");
        }
    }
}
