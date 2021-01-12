using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V1.Infrastructure.Migrations
{
    public partial class SORCodeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "m3nhfsor_code",
                table: "rate_schedule_items",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "m3nhfsor_code",
                table: "rate_schedule_items",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
