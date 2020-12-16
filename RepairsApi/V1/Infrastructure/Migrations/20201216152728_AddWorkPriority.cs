using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V1.Infrastructure.Migrations
{
    public partial class AddWorkPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "work_priority_code",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_priority_code", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "work_priority",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    priority_description = table.Column<string>(nullable: true),
                    comments = table.Column<string>(nullable: true),
                    PriorityCodeId = table.Column<int>(nullable: true),
                    number_of_days = table.Column<double>(nullable: false),
                    required_completion_datetime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_priority", x => x.id);
                    table.ForeignKey(
                        name: "FK_work_priority_work_priority_code_PriorityCodeId",
                        column: x => x.PriorityCodeId,
                        principalTable: "work_priority_code",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "work_priority_code",
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
                name: "IX_work_priority_PriorityCodeId",
                table: "work_priority",
                column: "PriorityCodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "work_priority");

            migrationBuilder.DropTable(
                name: "work_priority_code");
        }
    }
}
