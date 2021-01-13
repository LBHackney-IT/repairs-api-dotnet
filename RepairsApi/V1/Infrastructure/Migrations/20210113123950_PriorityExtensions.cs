using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V1.Infrastructure.Migrations
{
    public partial class PriorityExtensions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "comments",
                table: "work_priorities",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "number_of_days",
                table: "work_priorities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "priority_description",
                table: "work_priorities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comments",
                table: "work_priorities");

            migrationBuilder.DropColumn(
                name: "number_of_days",
                table: "work_priorities");

            migrationBuilder.DropColumn(
                name: "priority_description",
                table: "work_priorities");
        }
    }
}
