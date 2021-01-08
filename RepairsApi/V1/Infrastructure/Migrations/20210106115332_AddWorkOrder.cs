using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V1.Infrastructure.Migrations
{
    public partial class AddWorkOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkOrderId",
                table: "work_element",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "work_order",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'10000000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description_of_work = table.Column<string>(nullable: true),
                    PriorityId = table.Column<Guid>(nullable: true),
                    work_class_code = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_order", x => x.id);
                    table.ForeignKey(
                        name: "FK_work_order_work_priority_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "work_priority",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SitePropertyUnit",
                columns: table => new
                {
                    WorkOrderId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    reference = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitePropertyUnit", x => new { x.WorkOrderId, x.Id });
                    table.ForeignKey(
                        name: "FK_SitePropertyUnit_work_order_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "work_order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_work_element_WorkOrderId",
                table: "work_element",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_work_order_PriorityId",
                table: "work_order",
                column: "PriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_work_element_work_order_WorkOrderId",
                table: "work_element",
                column: "WorkOrderId",
                principalTable: "work_order",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_work_element_work_order_WorkOrderId",
                table: "work_element");

            migrationBuilder.DropTable(
                name: "SitePropertyUnit");

            migrationBuilder.DropTable(
                name: "work_order");

            migrationBuilder.DropIndex(
                name: "IX_work_element_WorkOrderId",
                table: "work_element");

            migrationBuilder.DropColumn(
                name: "WorkOrderId",
                table: "work_element");
        }
    }
}
