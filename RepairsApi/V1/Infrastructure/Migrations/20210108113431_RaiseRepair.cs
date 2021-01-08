using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V1.Infrastructure.Migrations
{
    public partial class RaiseRepair : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SitePropertyUnit_work_order_WorkOrderId",
                table: "SitePropertyUnit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SitePropertyUnit",
                table: "SitePropertyUnit");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SitePropertyUnit",
                newName: "id");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "SitePropertyUnit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "WorkOrderId",
                table: "SitePropertyUnit",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "address_line",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_building_name",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_building_number",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_city_name",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_complex_name",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "address_country",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_department",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_floor",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_plot",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_postal_code",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_postbox",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_room",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_street_name",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_type",
                table: "SitePropertyUnit",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SitePropertyUnit",
                table: "SitePropertyUnit",
                column: "id");

            migrationBuilder.CreateTable(
                name: "LocationAlerts",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comments = table.Column<string>(nullable: true),
                    type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationAlerts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PersonAlerts",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comments = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonAlerts", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SitePropertyUnit_WorkOrderId",
                table: "SitePropertyUnit",
                column: "WorkOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_SitePropertyUnit_work_order_WorkOrderId",
                table: "SitePropertyUnit",
                column: "WorkOrderId",
                principalTable: "work_order",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SitePropertyUnit_work_order_WorkOrderId",
                table: "SitePropertyUnit");

            migrationBuilder.DropTable(
                name: "LocationAlerts");

            migrationBuilder.DropTable(
                name: "PersonAlerts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SitePropertyUnit",
                table: "SitePropertyUnit");

            migrationBuilder.DropIndex(
                name: "IX_SitePropertyUnit_WorkOrderId",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_line",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_building_name",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_building_number",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_city_name",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_complex_name",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_country",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_department",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_floor",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_plot",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_postal_code",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_postbox",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_room",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_street_name",
                table: "SitePropertyUnit");

            migrationBuilder.DropColumn(
                name: "address_type",
                table: "SitePropertyUnit");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "SitePropertyUnit",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "WorkOrderId",
                table: "SitePropertyUnit",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "SitePropertyUnit",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SitePropertyUnit",
                table: "SitePropertyUnit",
                columns: new[] { "WorkOrderId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_SitePropertyUnit_work_order_WorkOrderId",
                table: "SitePropertyUnit",
                column: "WorkOrderId",
                principalTable: "work_order",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
