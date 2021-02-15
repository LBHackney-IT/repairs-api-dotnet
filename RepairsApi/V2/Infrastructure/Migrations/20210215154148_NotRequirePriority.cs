using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class NotRequirePriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sor_codes_sor_priorities_priority_id",
                table: "sor_codes");

            migrationBuilder.AddForeignKey(
                name: "fk_sor_codes_sor_priorities_priority_id",
                table: "sor_codes",
                column: "priority_id",
                principalTable: "sor_priorities",
                principalColumn: "priority_code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sor_codes_sor_priorities_priority_id",
                table: "sor_codes");

            migrationBuilder.AddForeignKey(
                name: "fk_sor_codes_sor_priorities_priority_id",
                table: "sor_codes",
                column: "priority_id",
                principalTable: "sor_priorities",
                principalColumn: "priority_code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
