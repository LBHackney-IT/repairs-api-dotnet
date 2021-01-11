using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V1.Infrastructure.Migrations
{
    public partial class RemovePriorityLookup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_work_priorities_work_priority_codes_priority_code_id",
                table: "work_priorities");

            migrationBuilder.DropTable(
                name: "work_priority_codes");

            migrationBuilder.DropIndex(
                name: "ix_work_priorities_priority_code_id",
                table: "work_priorities");

            migrationBuilder.DropColumn(
                name: "priority_code_id",
                table: "work_priorities");

            migrationBuilder.AddColumn<int>(
                name: "priority_code",
                table: "work_priorities",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "priority_code",
                table: "work_priorities");

            migrationBuilder.AddColumn<int>(
                name: "priority_code_id",
                table: "work_priorities",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "work_priority_codes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_priority_codes", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "work_priority_codes",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Emergency" },
                    { 2, "High" },
                    { 3, "Medium" },
                    { 4, "Low" },
                    { 5, "Deferred" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_work_priorities_priority_code_id",
                table: "work_priorities",
                column: "priority_code_id");

            migrationBuilder.AddForeignKey(
                name: "fk_work_priorities_work_priority_codes_priority_code_id",
                table: "work_priorities",
                column: "priority_code_id",
                principalTable: "work_priority_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
