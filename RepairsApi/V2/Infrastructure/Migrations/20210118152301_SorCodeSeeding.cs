using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class SorCodeSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sor_priorities",
                columns: table => new
                {
                    priority_code = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sor_priorities", x => x.priority_code);
                });

            migrationBuilder.CreateTable(
                name: "sor_codes",
                columns: table => new
                {
                    custom_code = table.Column<string>(type: "text", nullable: false),
                    custom_name = table.Column<string>(type: "text", nullable: true),
                    priority_id = table.Column<int>(type: "integer", nullable: false),
                    sor_contractor_ref = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sor_codes", x => x.custom_code);
                    table.ForeignKey(
                        name: "fk_sor_codes_sor_priorities_priority_id",
                        column: x => x.priority_id,
                        principalTable: "sor_priorities",
                        principalColumn: "priority_code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "sor_priorities",
                columns: new[] { "priority_code", "description" },
                values: new object[,]
                {
                    { 1, "I - Immediate (2 hours)" },
                    { 2, "E - Emergency (24 hours)" },
                    { 3, "U - Urgent (5 Working days)" },
                    { 4, "N - Normal (21 working days)" },
                    { 5, "Inspection" }
                });

            migrationBuilder.InsertData(
                table: "sor_codes",
                columns: new[] { "custom_code", "custom_name", "priority_id", "sor_contractor_ref" },
                values: new object[,]
                {
                    { "DES5R003", "Immediate call outs", 1, "H01" },
                    { "DES5R004", "Emergency call out", 2, "H01" },
                    { "DES5R006", "Urgent call outs", 3, "H01" },
                    { "DES5R005", "Normal call outs", 4, "H01" },
                    { "DES5R013", "Inspect additional sec entrance", 5, "H01" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_sor_codes_priority_id",
                table: "sor_codes",
                column: "priority_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sor_codes");

            migrationBuilder.DropTable(
                name: "sor_priorities");
        }
    }
}
