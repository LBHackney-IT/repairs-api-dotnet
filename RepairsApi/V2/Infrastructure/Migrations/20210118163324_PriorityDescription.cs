using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class PriorityDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "sor_priorities",
                keyColumn: "priority_code",
                keyValue: 3,
                column: "description",
                value: "U - Urgent 7 days (5 Working days)");

            migrationBuilder.UpdateData(
                table: "sor_priorities",
                keyColumn: "priority_code",
                keyValue: 4,
                column: "description",
                value: "N - Normal 28 days (21 working days)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "sor_priorities",
                keyColumn: "priority_code",
                keyValue: 3,
                column: "description",
                value: "U - Urgent (5 Working days)");

            migrationBuilder.UpdateData(
                table: "sor_priorities",
                keyColumn: "priority_code",
                keyValue: 4,
                column: "description",
                value: "N - Normal (21 working days)");
        }
    }
}
