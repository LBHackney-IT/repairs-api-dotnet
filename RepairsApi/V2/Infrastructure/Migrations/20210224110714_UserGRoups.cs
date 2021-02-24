using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class UserGRoups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "cost",
                table: "sor_contracts",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<double>(
                name: "original_quantity",
                table: "rate_schedule_item",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "security_groups",
                columns: table => new
                {
                    group_name = table.Column<string>(type: "text", nullable: false),
                    user_type = table.Column<string>(type: "text", nullable: true),
                    contractor_reference = table.Column<string>(type: "text", nullable: true),
                    raise_limit = table.Column<double>(type: "double precision", nullable: true),
                    vary_limit = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_security_groups", x => x.group_name);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "security_groups");

            migrationBuilder.DropColumn(
                name: "original_quantity",
                table: "rate_schedule_item");

            migrationBuilder.AlterColumn<double>(
                name: "cost",
                table: "sor_contracts",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }
    }
}
