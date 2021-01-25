using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class ScheduleRepair : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_property_address_postal_address_address_id",
                table: "property_address");

            migrationBuilder.DropForeignKey(
                name: "fk_property_class_geographical_location_geographical_location_",
                table: "property_class");

            migrationBuilder.DropForeignKey(
                name: "fk_site_geographical_location_geographical_location_id",
                table: "site");

            migrationBuilder.DropTable(
                name: "point");

            migrationBuilder.DropTable(
                name: "postal_address");

            migrationBuilder.DropTable(
                name: "geographical_location");

            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropIndex(
                name: "ix_site_geographical_location_id",
                table: "site");

            migrationBuilder.DropIndex(
                name: "ix_property_class_geographical_location_id",
                table: "property_class");

            migrationBuilder.DropIndex(
                name: "ix_property_address_address_id",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "geographical_location_id",
                table: "site");

            migrationBuilder.DropColumn(
                name: "geographical_location_id",
                table: "property_class");

            migrationBuilder.RenameColumn(
                name: "address_id",
                table: "property_address",
                newName: "country");

            migrationBuilder.AddColumn<int>(
                name: "assigned_to_primary_id",
                table: "work_orders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "customer_id",
                table: "work_orders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "instructed_by_id",
                table: "work_orders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "geographical_location_elevation",
                table: "site",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "geographical_location_elevation_reference_system",
                table: "site",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "geographical_location_latitude",
                table: "site",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "geographical_location_longitude",
                table: "site",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "geographical_location_polyline",
                table: "site",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "geographical_location_positional_accuracy",
                table: "site",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "geographical_location_elevation",
                table: "property_class",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "geographical_location_elevation_reference_system",
                table: "property_class",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "geographical_location_latitude",
                table: "property_class",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "geographical_location_longitude",
                table: "property_class",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "geographical_location_polyline",
                table: "property_class",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "geographical_location_positional_accuracy",
                table: "property_class",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_line",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "building_name",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "building_number",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "city_name",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "complex_name",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "contact_id",
                table: "property_address",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "department",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "floor",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "plot",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "postal_code",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "postbox",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "room",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "street_name",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "property_address",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name_family",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name_family_prefix",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name_given",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name_initials",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name_middle",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name_title",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "organization_id",
                table: "party",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "person_id",
                table: "party",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customer_communication_channel_attempted_description",
                table: "job_status_updates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "customer_communication_channel_attempted_not_available",
                table: "job_status_updates",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "organization",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    doing_business_as_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "person_AliasNames",
                columns: table => new
                {
                    person_id = table.Column<int>(type: "integer", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full = table.Column<string>(type: "text", nullable: true),
                    given = table.Column<string>(type: "text", nullable: true),
                    family = table.Column<string>(type: "text", nullable: true),
                    family_prefix = table.Column<string>(type: "text", nullable: true),
                    initials = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "text", nullable: true),
                    middle = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_alias_names", x => new { x.person_id, x.id });
                    table.ForeignKey(
                        name: "fk_person_alias_names_person_person_id",
                        column: x => x.person_id,
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "person_Communication",
                columns: table => new
                {
                    person_id = table.Column<int>(type: "integer", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    channel_medium = table.Column<int>(type: "integer", nullable: true),
                    channel_code = table.Column<int>(type: "integer", nullable: true),
                    value = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    not_available = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_communication", x => new { x.person_id, x.id });
                    table.ForeignKey(
                        name: "fk_person_communication_person_person_id",
                        column: x => x.person_id,
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    person_id = table.Column<int>(type: "integer", nullable: true),
                    organization_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contact", x => x.id);
                    table.ForeignKey(
                        name: "fk_contact_organization_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_contact_person_person_id",
                        column: x => x.person_id,
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_assigned_to_primary_id",
                table: "work_orders",
                column: "assigned_to_primary_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_customer_id",
                table: "work_orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_instructed_by_id",
                table: "work_orders",
                column: "instructed_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_property_address_contact_id",
                table: "property_address",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "ix_party_organization_id",
                table: "party",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_party_person_id",
                table: "party",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_contact_organization_id",
                table: "contact",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_contact_person_id",
                table: "contact",
                column: "person_id");

            migrationBuilder.AddForeignKey(
                name: "fk_party_organization_organization_id",
                table: "party",
                column: "organization_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_party_person_person_id",
                table: "party",
                column: "person_id",
                principalTable: "person",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_property_address_contact_contact_id",
                table: "property_address",
                column: "contact_id",
                principalTable: "contact",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_orders_organization_instructed_by_id",
                table: "work_orders",
                column: "instructed_by_id",
                principalTable: "organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_orders_party_assigned_to_primary_id",
                table: "work_orders",
                column: "assigned_to_primary_id",
                principalTable: "party",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_orders_party_customer_id",
                table: "work_orders",
                column: "customer_id",
                principalTable: "party",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_party_organization_organization_id",
                table: "party");

            migrationBuilder.DropForeignKey(
                name: "fk_party_person_person_id",
                table: "party");

            migrationBuilder.DropForeignKey(
                name: "fk_property_address_contact_contact_id",
                table: "property_address");

            migrationBuilder.DropForeignKey(
                name: "fk_work_orders_organization_instructed_by_id",
                table: "work_orders");

            migrationBuilder.DropForeignKey(
                name: "fk_work_orders_party_assigned_to_primary_id",
                table: "work_orders");

            migrationBuilder.DropForeignKey(
                name: "fk_work_orders_party_customer_id",
                table: "work_orders");

            migrationBuilder.DropTable(
                name: "contact");

            migrationBuilder.DropTable(
                name: "person_AliasNames");

            migrationBuilder.DropTable(
                name: "person_Communication");

            migrationBuilder.DropTable(
                name: "organization");

            migrationBuilder.DropIndex(
                name: "ix_work_orders_assigned_to_primary_id",
                table: "work_orders");

            migrationBuilder.DropIndex(
                name: "ix_work_orders_customer_id",
                table: "work_orders");

            migrationBuilder.DropIndex(
                name: "ix_work_orders_instructed_by_id",
                table: "work_orders");

            migrationBuilder.DropIndex(
                name: "ix_property_address_contact_id",
                table: "property_address");

            migrationBuilder.DropIndex(
                name: "ix_party_organization_id",
                table: "party");

            migrationBuilder.DropIndex(
                name: "ix_party_person_id",
                table: "party");

            migrationBuilder.DropColumn(
                name: "assigned_to_primary_id",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "instructed_by_id",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "geographical_location_elevation",
                table: "site");

            migrationBuilder.DropColumn(
                name: "geographical_location_elevation_reference_system",
                table: "site");

            migrationBuilder.DropColumn(
                name: "geographical_location_latitude",
                table: "site");

            migrationBuilder.DropColumn(
                name: "geographical_location_longitude",
                table: "site");

            migrationBuilder.DropColumn(
                name: "geographical_location_polyline",
                table: "site");

            migrationBuilder.DropColumn(
                name: "geographical_location_positional_accuracy",
                table: "site");

            migrationBuilder.DropColumn(
                name: "geographical_location_elevation",
                table: "property_class");

            migrationBuilder.DropColumn(
                name: "geographical_location_elevation_reference_system",
                table: "property_class");

            migrationBuilder.DropColumn(
                name: "geographical_location_latitude",
                table: "property_class");

            migrationBuilder.DropColumn(
                name: "geographical_location_longitude",
                table: "property_class");

            migrationBuilder.DropColumn(
                name: "geographical_location_polyline",
                table: "property_class");

            migrationBuilder.DropColumn(
                name: "geographical_location_positional_accuracy",
                table: "property_class");

            migrationBuilder.DropColumn(
                name: "address_line",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "building_name",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "building_number",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "city_name",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "complex_name",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "contact_id",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "department",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "floor",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "plot",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "postal_code",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "postbox",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "room",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "street_name",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "type",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "name_family",
                table: "person");

            migrationBuilder.DropColumn(
                name: "name_family_prefix",
                table: "person");

            migrationBuilder.DropColumn(
                name: "name_given",
                table: "person");

            migrationBuilder.DropColumn(
                name: "name_initials",
                table: "person");

            migrationBuilder.DropColumn(
                name: "name_middle",
                table: "person");

            migrationBuilder.DropColumn(
                name: "name_title",
                table: "person");

            migrationBuilder.DropColumn(
                name: "organization_id",
                table: "party");

            migrationBuilder.DropColumn(
                name: "person_id",
                table: "party");

            migrationBuilder.DropColumn(
                name: "customer_communication_channel_attempted_description",
                table: "job_status_updates");

            migrationBuilder.DropColumn(
                name: "customer_communication_channel_attempted_not_available",
                table: "job_status_updates");

            migrationBuilder.RenameColumn(
                name: "country",
                table: "property_address",
                newName: "address_id");

            migrationBuilder.AddColumn<int>(
                name: "geographical_location_id",
                table: "site",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "geographical_location_id",
                table: "property_class",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address_line = table.Column<string>(type: "text", nullable: true),
                    building_name = table.Column<string>(type: "text", nullable: true),
                    building_number = table.Column<string>(type: "text", nullable: true),
                    city_name = table.Column<string>(type: "text", nullable: true),
                    complex_name = table.Column<string>(type: "text", nullable: true),
                    country = table.Column<int>(type: "integer", nullable: true),
                    department = table.Column<string>(type: "text", nullable: true),
                    floor = table.Column<string>(type: "text", nullable: true),
                    plot = table.Column<string>(type: "text", nullable: true),
                    postal_code = table.Column<string>(type: "text", nullable: true),
                    postbox = table.Column<string>(type: "text", nullable: true),
                    room = table.Column<string>(type: "text", nullable: true),
                    street_name = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_address", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "geographical_location",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    elevation = table.Column<double>(type: "double precision", nullable: true),
                    elevation_reference_system = table.Column<string>(type: "text", nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    positional_accuracy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_geographical_location", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "postal_address",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_postal_address", x => x.id);
                    table.ForeignKey(
                        name: "fk_postal_address_address_address_id",
                        column: x => x.address_id,
                        principalTable: "address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "point",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    easting = table.Column<double>(type: "double precision", nullable: true),
                    geographical_location_id = table.Column<int>(type: "integer", nullable: true),
                    northing = table.Column<double>(type: "double precision", nullable: true),
                    sequence = table.Column<int>(type: "integer", nullable: false)
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
                name: "ix_property_address_address_id",
                table: "property_address",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "ix_point_geographical_location_id",
                table: "point",
                column: "geographical_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_postal_address_address_id",
                table: "postal_address",
                column: "address_id");

            migrationBuilder.AddForeignKey(
                name: "fk_property_address_postal_address_address_id",
                table: "property_address",
                column: "address_id",
                principalTable: "postal_address",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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
    }
}
