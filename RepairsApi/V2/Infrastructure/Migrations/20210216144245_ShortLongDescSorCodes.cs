using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class ShortLongDescSorCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sor_contracts_sor_codes_sor_code_custom_code",
                table: "sor_contracts");

            migrationBuilder.RenameColumn(
                name: "custom_name",
                table: "sor_codes",
                newName: "short_description");

            migrationBuilder.RenameColumn(
                name: "custom_code",
                table: "sor_codes",
                newName: "code");

            migrationBuilder.AddColumn<string>(
                name: "long_description",
                table: "sor_codes",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_sor_contracts_sor_codes_sor_code_code",
                table: "sor_contracts",
                column: "sor_code_code",
                principalTable: "sor_codes",
                principalColumn: "code",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sor_contracts_sor_codes_sor_code_code",
                table: "sor_contracts");

            migrationBuilder.DropColumn(
                name: "long_description",
                table: "sor_codes");

            migrationBuilder.RenameColumn(
                name: "short_description",
                table: "sor_codes",
                newName: "custom_name");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "sor_codes",
                newName: "custom_code");

            migrationBuilder.AddForeignKey(
                name: "fk_sor_contracts_sor_codes_sor_code_custom_code",
                table: "sor_contracts",
                column: "sor_code_code",
                principalTable: "sor_codes",
                principalColumn: "custom_code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
