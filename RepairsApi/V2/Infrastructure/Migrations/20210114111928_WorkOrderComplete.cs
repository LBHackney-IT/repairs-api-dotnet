using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class WorkOrderComplete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "geographical_location_id",
                table: "site",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "site",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "geographical_location_id",
                table: "property_class",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "master_key_system",
                table: "property_class",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "geographical_location",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    longitude = table.Column<double>(nullable: true),
                    latitude = table.Column<double>(nullable: true),
                    elevation = table.Column<double>(nullable: true),
                    elevation_reference_system = table.Column<string>(nullable: true),
                    positional_accuracy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_geographical_location", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "point",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sequence = table.Column<int>(nullable: false),
                    easting = table.Column<double>(nullable: true),
                    northing = table.Column<double>(nullable: true),
                    geographical_location_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_point", x => x.id);
                    table.ForeignKey(
                        name: "fk_point_geographical_location_geographical_location_id",
                        column: x => x.geographical_location_id,
                        principalTable: "geographical_location",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_site_geographical_location_id",
                table: "site",
                column: "geographical_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_property_class_geographical_location_id",
                table: "property_class",
                column: "geographical_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_point_geographical_location_id",
                table: "point",
                column: "geographical_location_id");

            migrationBuilder.AddForeignKey(
                name: "fk_property_class_geographical_location_geographical_location_",
                table: "property_class",
                column: "geographical_location_id",
                principalTable: "geographical_location",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_site_geographical_location_geographical_location_id",
                table: "site",
                column: "geographical_location_id",
                principalTable: "geographical_location",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_property_class_geographical_location_geographical_location_",
                table: "property_class");

            migrationBuilder.DropForeignKey(
                name: "fk_site_geographical_location_geographical_location_id",
                table: "site");

            migrationBuilder.DropTable(
                name: "point");

            migrationBuilder.DropTable(
                name: "geographical_location");

            migrationBuilder.DropIndex(
                name: "ix_site_geographical_location_id",
                table: "site");

            migrationBuilder.DropIndex(
                name: "ix_property_class_geographical_location_id",
                table: "property_class");

            migrationBuilder.DropColumn(
                name: "geographical_location_id",
                table: "site");

            migrationBuilder.DropColumn(
                name: "name",
                table: "site");

            migrationBuilder.DropColumn(
                name: "geographical_location_id",
                table: "property_class");

            migrationBuilder.DropColumn(
                name: "master_key_system",
                table: "property_class");
        }
    }
}
