using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class RemoveUnusedTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_job_status_updates_customer_satisfaction_customer_feedback_",
                table: "job_status_updates");

            migrationBuilder.DropForeignKey(
                name: "fk_property_address_contact_contact_id",
                table: "property_address");

            migrationBuilder.DropTable(
                name: "alert_regarding_location");

            migrationBuilder.DropTable(
                name: "alert_regarding_person");

            migrationBuilder.DropTable(
                name: "categorization");

            migrationBuilder.DropTable(
                name: "contact");

            migrationBuilder.DropTable(
                name: "score");

            migrationBuilder.DropTable(
                name: "work_element_dependency");

            migrationBuilder.DropTable(
                name: "score_set");

            migrationBuilder.DropTable(
                name: "customer_satisfaction");

            migrationBuilder.DropIndex(
                name: "ix_property_address_contact_id",
                table: "property_address");

            migrationBuilder.DropIndex(
                name: "ix_job_status_updates_customer_feedback_id",
                table: "job_status_updates");

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
                name: "contact_id",
                table: "property_address");

            migrationBuilder.DropColumn(
                name: "customer_feedback_id",
                table: "job_status_updates");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<int>(
                name: "contact_id",
                table: "property_address",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "customer_feedback_id",
                table: "job_status_updates",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "alert_regarding_location",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comments = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: true),
                    work_order_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_alert_regarding_location", x => x.id);
                    table.ForeignKey(
                        name: "fk_alert_regarding_location_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "alert_regarding_person",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comments = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: true),
                    work_order_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_alert_regarding_person", x => x.id);
                    table.ForeignKey(
                        name: "fk_alert_regarding_person_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contact",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organization_id = table.Column<int>(type: "integer", nullable: true),
                    person_id = table.Column<int>(type: "integer", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "customer_satisfaction",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    party_carrying_out_survey_id = table.Column<int>(type: "integer", nullable: true),
                    party_providing_feedback_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_satisfaction", x => x.id);
                    table.ForeignKey(
                        name: "fk_customer_satisfaction_party_party_carrying_out_survey_id",
                        column: x => x.party_carrying_out_survey_id,
                        principalTable: "party",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_customer_satisfaction_party_party_providing_feedback_id",
                        column: x => x.party_providing_feedback_id,
                        principalTable: "party",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_element_dependency",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    depends_on_work_element_id = table.Column<Guid>(type: "uuid", nullable: true),
                    dependency_duration = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    dependency_type = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_element_dependency", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_element_dependency_work_elements_depends_on_work_eleme",
                        column: x => x.depends_on_work_element_id,
                        principalTable: "work_elements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "score_set",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_satisfaction_id = table.Column<int>(type: "integer", nullable: true),
                    date_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    previous_date_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_score_set", x => x.id);
                    table.ForeignKey(
                        name: "fk_score_set_customer_satisfaction_customer_satisfaction_id",
                        column: x => x.customer_satisfaction_id,
                        principalTable: "customer_satisfaction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "categorization",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category = table.Column<string>(type: "text", nullable: true),
                    score_set_id = table.Column<int>(type: "integer", nullable: true),
                    sub_category = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true),
                    version_used = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categorization", x => x.id);
                    table.ForeignKey(
                        name: "fk_categorization_score_set_score_set_id",
                        column: x => x.score_set_id,
                        principalTable: "score_set",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "score",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comment = table.Column<string>(type: "text", nullable: true),
                    current_score = table.Column<string>(type: "text", nullable: true),
                    follow_up_question = table.Column<string>(type: "text", nullable: true),
                    maximum = table.Column<string>(type: "text", nullable: true),
                    minimum = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    score_set_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_score", x => x.id);
                    table.ForeignKey(
                        name: "fk_score_score_set_score_set_id",
                        column: x => x.score_set_id,
                        principalTable: "score_set",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_property_address_contact_id",
                table: "property_address",
                column: "contact_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_status_updates_customer_feedback_id",
                table: "job_status_updates",
                column: "customer_feedback_id");

            migrationBuilder.CreateIndex(
                name: "ix_alert_regarding_location_work_order_id",
                table: "alert_regarding_location",
                column: "work_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_alert_regarding_person_work_order_id",
                table: "alert_regarding_person",
                column: "work_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_categorization_score_set_id",
                table: "categorization",
                column: "score_set_id");

            migrationBuilder.CreateIndex(
                name: "ix_contact_organization_id",
                table: "contact",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_contact_person_id",
                table: "contact",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_satisfaction_party_carrying_out_survey_id",
                table: "customer_satisfaction",
                column: "party_carrying_out_survey_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_satisfaction_party_providing_feedback_id",
                table: "customer_satisfaction",
                column: "party_providing_feedback_id");

            migrationBuilder.CreateIndex(
                name: "ix_score_score_set_id",
                table: "score",
                column: "score_set_id");

            migrationBuilder.CreateIndex(
                name: "ix_score_set_customer_satisfaction_id",
                table: "score_set",
                column: "customer_satisfaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_element_dependency_depends_on_work_element_id",
                table: "work_element_dependency",
                column: "depends_on_work_element_id");

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_updates_customer_satisfaction_customer_feedback_",
                table: "job_status_updates",
                column: "customer_feedback_id",
                principalTable: "customer_satisfaction",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_property_address_contact_contact_id",
                table: "property_address",
                column: "contact_id",
                principalTable: "contact",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
