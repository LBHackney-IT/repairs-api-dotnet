using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class ContractSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                keyValue: "DES5R003");

            migrationBuilder.DeleteData(
                table: "sor_codes",
                keyColumn: "custom_code",
                keyValue: "DES5R004");

            migrationBuilder.DeleteData(
                table: "sor_codes",
                keyColumn: "custom_code",
                keyValue: "DES5R005");

            migrationBuilder.DeleteData(
                table: "sor_codes",
                keyColumn: "custom_code",
                keyValue: "DES5R006");

            migrationBuilder.DeleteData(
                table: "sor_codes",
                keyColumn: "custom_code",
                keyValue: "DES5R013");

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

            migrationBuilder.DeleteData(
                table: "sor_priorities",
                keyColumn: "priority_code",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "sor_priorities",
                keyColumn: "priority_code",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "sor_priorities",
                keyColumn: "priority_code",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "sor_priorities",
                keyColumn: "priority_code",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "sor_priorities",
                keyColumn: "priority_code",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "sor_contractor_ref",
                table: "sor_codes");

            migrationBuilder.AlterColumn<double>(
                name: "cost",
                table: "sor_codes",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<string>(
                name: "trade_code",
                table: "sor_codes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "contractor_reference",
                table: "party",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "contractors",
                columns: table => new
                {
                    reference = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contractors", x => x.reference);
                });

            migrationBuilder.CreateTable(
                name: "trades",
                columns: table => new
                {
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trades", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                columns: table => new
                {
                    contract_reference = table.Column<string>(type: "text", nullable: false),
                    termination_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    effective_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    contractor_reference = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contracts", x => x.contract_reference);
                    table.ForeignKey(
                        name: "fk_contracts_contractors_contractor_reference",
                        column: x => x.contractor_reference,
                        principalTable: "contractors",
                        principalColumn: "reference",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "property_contracts",
                columns: table => new
                {
                    prop_ref = table.Column<string>(type: "text", nullable: false),
                    contract_reference = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_property_contracts", x => new { x.contract_reference, x.prop_ref });
                    table.ForeignKey(
                        name: "fk_property_contracts_contracts_contract_reference1",
                        column: x => x.contract_reference,
                        principalTable: "contracts",
                        principalColumn: "contract_reference",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sor_contracts",
                columns: table => new
                {
                    sor_code_code = table.Column<string>(type: "text", nullable: false),
                    contract_reference = table.Column<string>(type: "text", nullable: false),
                    cost = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sor_contracts", x => new { x.contract_reference, x.sor_code_code });
                    table.ForeignKey(
                        name: "fk_sor_contracts_contracts_contract_reference1",
                        column: x => x.contract_reference,
                        principalTable: "contracts",
                        principalColumn: "contract_reference",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sor_contracts_sor_codes_sor_code_custom_code",
                        column: x => x.sor_code_code,
                        principalTable: "sor_codes",
                        principalColumn: "custom_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sor_codes_trade_code",
                table: "sor_codes",
                column: "trade_code");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_contractor_reference",
                table: "contracts",
                column: "contractor_reference");

            migrationBuilder.CreateIndex(
                name: "ix_sor_contracts_sor_code_code",
                table: "sor_contracts",
                column: "sor_code_code");

            migrationBuilder.AddForeignKey(
                name: "fk_sor_codes_sor_priorities_priority_id",
                table: "sor_codes",
                column: "priority_id",
                principalTable: "sor_priorities",
                principalColumn: "priority_code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_sor_codes_trades_trade_code",
                table: "sor_codes",
                column: "trade_code",
                principalTable: "trades",
                principalColumn: "code",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sor_codes_sor_priorities_priority_id",
                table: "sor_codes");

            migrationBuilder.DropForeignKey(
                name: "fk_sor_codes_trades_trade_code",
                table: "sor_codes");

            migrationBuilder.DropTable(
                name: "property_contracts");

            migrationBuilder.DropTable(
                name: "sor_contracts");

            migrationBuilder.DropTable(
                name: "trades");

            migrationBuilder.DropTable(
                name: "contracts");

            migrationBuilder.DropTable(
                name: "contractors");

            migrationBuilder.DropIndex(
                name: "ix_sor_codes_trade_code",
                table: "sor_codes");

            migrationBuilder.DropColumn(
                name: "trade_code",
                table: "sor_codes");

            migrationBuilder.DropColumn(
                name: "contractor_reference",
                table: "party");

            migrationBuilder.AlterColumn<double>(
                name: "cost",
                table: "sor_codes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sor_contractor_ref",
                table: "sor_codes",
                type: "text",
                nullable: true);

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

            migrationBuilder.InsertData(
                table: "sor_priorities",
                columns: new[] { "priority_code", "description" },
                values: new object[,]
                {
                    { 1, "I - Immediate (2 hours)" },
                    { 2, "E - Emergency (24 hours)" },
                    { 3, "U - Urgent 7 days (5 Working days)" },
                    { 4, "N - Normal 28 days (21 working days)" },
                    { 5, "Inspection" }
                });

            migrationBuilder.InsertData(
                table: "sor_codes",
                columns: new[] { "custom_code", "cost", "custom_name", "priority_id", "sor_contractor_ref" },
                values: new object[,]
                {
                    { "DES5R003", 0.0, "Immediate call outs", 1, "H01" },
                    { "DES5R004", 0.0, "Emergency call out", 2, "H01" },
                    { "DES5R006", 0.0, "Urgent call outs", 3, "H01" },
                    { "DES5R005", 0.0, "Normal call outs", 4, "H01" },
                    { "DES5R013", 0.0, "Inspect additional sec entrance", 5, "H01" }
                });

            migrationBuilder.AddForeignKey(
                name: "fk_sor_codes_sor_priorities_priority_id",
                table: "sor_codes",
                column: "priority_id",
                principalTable: "sor_priorities",
                principalColumn: "priority_code",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
