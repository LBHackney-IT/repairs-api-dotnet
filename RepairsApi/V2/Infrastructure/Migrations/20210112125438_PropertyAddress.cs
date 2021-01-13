using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class PropertyAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "address_id",
                table: "property_class",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "country",
                table: "address",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "ix_property_class_address_id",
                table: "property_class",
                column: "address_id");

            migrationBuilder.AddForeignKey(
                name: "fk_property_class_property_address_address_id",
                table: "property_class",
                column: "address_id",
                principalTable: "property_address",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_property_class_property_address_address_id",
                table: "property_class");

            migrationBuilder.DropIndex(
                name: "ix_property_class_address_id",
                table: "property_class");

            migrationBuilder.DropColumn(
                name: "address_id",
                table: "property_class");

            migrationBuilder.AlterColumn<int>(
                name: "country",
                table: "address",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
