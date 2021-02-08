using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class PurdyCodesSeeded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sor_codes_sor_priorities_priority_id",
                table: "sor_codes");

            migrationBuilder.AlterColumn<int>(
                name: "priority_id",
                table: "sor_codes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                table: "sor_codes",
                columns: new[] { "custom_code", "cost", "custom_name", "priority_id", "sor_contractor_ref" },
                values: new object[,]
                {
                    { "INP5R001", 0.0, "Pre insp of wrks by Constructr", null, "G16-PCL-PGBR" },
                    { "CPP5R056", 0.0, "EASE  ADJUST DOORWINDOW", null, "G16-PCL-PGBR" },
                    { "PLP5R054", 0.0, "RENEW MIXER TAP", null, "G16-PCL-PGBR" },
                    { "SCP5V055", 0.0, "ROPE ACCESS FOR 6 STOREY", null, "G16-PCL-PGBR" },
                    { "ELP5R002", 0.0, "CASH ITEM VOID - MATS EXT OVER", null, "G16-PCL-PGBR" }
                });

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

            migrationBuilder.DeleteData(
                table: "sor_codes",
                keyColumn: "custom_code",
                keyValue: "CPP5R056");

            migrationBuilder.DeleteData(
                table: "sor_codes",
                keyColumn: "custom_code",
                keyValue: "ELP5R002");

            migrationBuilder.DeleteData(
                table: "sor_codes",
                keyColumn: "custom_code",
                keyValue: "INP5R001");

            migrationBuilder.DeleteData(
                table: "sor_codes",
                keyColumn: "custom_code",
                keyValue: "PLP5R054");

            migrationBuilder.DeleteData(
                table: "sor_codes",
                keyColumn: "custom_code",
                keyValue: "SCP5V055");

            migrationBuilder.AlterColumn<int>(
                name: "priority_id",
                table: "sor_codes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

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
