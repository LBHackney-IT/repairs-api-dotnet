using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class WorkOrderComplete2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "work_order_complete_id",
                table: "work_orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "job_status_update_id",
                table: "work_elements",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "operative_id",
                table: "work_elements",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "work_order_complete_id",
                table: "work_elements",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "operative_id",
                table: "trade",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "work_order_complete_id",
                table: "rate_schedule_items",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "additional_work",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    additional_work_order_id = table.Column<int>(nullable: true),
                    reason_required = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_additional_work", x => x.id);
                    table.ForeignKey(
                        name: "fk_additional_work_work_orders_additional_work_order_id",
                        column: x => x.additional_work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "appointment_time_of_day",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true),
                    earliest_arrival_time = table.Column<DateTime>(nullable: true),
                    latest_arrival_time = table.Column<DateTime>(nullable: true),
                    latest_completion_time = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointment_time_of_day", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "communication_channel",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    medium = table.Column<int>(nullable: true),
                    code = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_communication_channel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "identification",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(nullable: true),
                    number = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identification", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "party",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true),
                    role = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_party", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "person_name",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_name", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "work_order_completes",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    work_order_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_order_completes", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_order_completes_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "appointment",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateTime>(nullable: true),
                    time_of_day_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointment", x => x.id);
                    table.ForeignKey(
                        name: "fk_appointment_appointment_time_of_day_time_of_day_id",
                        column: x => x.time_of_day_id,
                        principalTable: "appointment_time_of_day",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "communication",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    channel_id = table.Column<int>(nullable: true),
                    value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_communication", x => x.id);
                    table.ForeignKey(
                        name: "fk_communication_communication_channel_channel_id",
                        column: x => x.channel_id,
                        principalTable: "communication_channel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "customer_satisfaction",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    party_providing_feedback_id = table.Column<int>(nullable: true),
                    party_carrying_out_survey_id = table.Column<int>(nullable: true)
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
                name: "job_status_update",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_time = table.Column<DateTime>(nullable: true),
                    type_code = table.Column<int>(nullable: true),
                    other_type = table.Column<string>(nullable: true),
                    refined_appointment_window_id = table.Column<int>(nullable: true),
                    customer_feedback_id = table.Column<int>(nullable: true),
                    customer_communication_channel_attempted_id = table.Column<int>(nullable: true),
                    comments = table.Column<string>(nullable: true),
                    more_specific_sor_code_id = table.Column<Guid>(nullable: true),
                    additional_work_id = table.Column<int>(nullable: true),
                    related_work_order_id = table.Column<int>(nullable: true),
                    work_order_complete_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_status_update", x => x.id);
                    table.ForeignKey(
                        name: "fk_job_status_update_additional_work_additional_work_id",
                        column: x => x.additional_work_id,
                        principalTable: "additional_work",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_job_status_update_communication_customer_communication_chan",
                        column: x => x.customer_communication_channel_attempted_id,
                        principalTable: "communication",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_job_status_update_customer_satisfaction_customer_feedback_id",
                        column: x => x.customer_feedback_id,
                        principalTable: "customer_satisfaction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_job_status_update_work_elements_more_specific_sor_code_id",
                        column: x => x.more_specific_sor_code_id,
                        principalTable: "work_elements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_job_status_update_appointment_refined_appointment_window_id",
                        column: x => x.refined_appointment_window_id,
                        principalTable: "appointment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_job_status_update_work_orders_related_work_order_id",
                        column: x => x.related_work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_job_status_update_work_order_completes_work_order_complete_",
                        column: x => x.work_order_complete_id,
                        principalTable: "work_order_completes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "score_set",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_time = table.Column<DateTime>(nullable: true),
                    previous_date_time = table.Column<DateTime>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    customer_satisfaction_id = table.Column<int>(nullable: true)
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
                name: "person",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name_id = table.Column<int>(nullable: true),
                    identification_id = table.Column<int>(nullable: true),
                    job_status_update_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person", x => x.id);
                    table.ForeignKey(
                        name: "fk_person_identification_identification_id",
                        column: x => x.identification_id,
                        principalTable: "identification",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_person_job_status_update_job_status_update_id",
                        column: x => x.job_status_update_id,
                        principalTable: "job_status_update",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_person_person_name_name_id",
                        column: x => x.name_id,
                        principalTable: "person_name",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "categorization",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(nullable: true),
                    category = table.Column<string>(nullable: true),
                    version_used = table.Column<string>(nullable: true),
                    sub_category = table.Column<string>(nullable: true),
                    score_set_id = table.Column<int>(nullable: true)
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
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: true),
                    current_score = table.Column<string>(nullable: true),
                    minimum = table.Column<string>(nullable: true),
                    maximum = table.Column<string>(nullable: true),
                    follow_up_question = table.Column<string>(nullable: true),
                    comment = table.Column<string>(nullable: true),
                    score_set_id = table.Column<int>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "operative",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    person_id = table.Column<int>(nullable: true),
                    work_order_complete_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_operative", x => x.id);
                    table.ForeignKey(
                        name: "fk_operative_person_person_id",
                        column: x => x.person_id,
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_operative_work_order_completes_work_order_complete_id",
                        column: x => x.work_order_complete_id,
                        principalTable: "work_order_completes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_work_order_complete_id",
                table: "work_orders",
                column: "work_order_complete_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_elements_job_status_update_id",
                table: "work_elements",
                column: "job_status_update_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_elements_operative_id",
                table: "work_elements",
                column: "operative_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_elements_work_order_complete_id",
                table: "work_elements",
                column: "work_order_complete_id");

            migrationBuilder.CreateIndex(
                name: "ix_trade_operative_id",
                table: "trade",
                column: "operative_id");

            migrationBuilder.CreateIndex(
                name: "ix_rate_schedule_items_work_order_complete_id",
                table: "rate_schedule_items",
                column: "work_order_complete_id");

            migrationBuilder.CreateIndex(
                name: "ix_additional_work_additional_work_order_id",
                table: "additional_work",
                column: "additional_work_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_time_of_day_id",
                table: "appointment",
                column: "time_of_day_id");

            migrationBuilder.CreateIndex(
                name: "ix_categorization_score_set_id",
                table: "categorization",
                column: "score_set_id");

            migrationBuilder.CreateIndex(
                name: "ix_communication_channel_id",
                table: "communication",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_satisfaction_party_carrying_out_survey_id",
                table: "customer_satisfaction",
                column: "party_carrying_out_survey_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_satisfaction_party_providing_feedback_id",
                table: "customer_satisfaction",
                column: "party_providing_feedback_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_status_update_additional_work_id",
                table: "job_status_update",
                column: "additional_work_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_status_update_customer_communication_channel_attempted_",
                table: "job_status_update",
                column: "customer_communication_channel_attempted_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_status_update_customer_feedback_id",
                table: "job_status_update",
                column: "customer_feedback_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_status_update_more_specific_sor_code_id",
                table: "job_status_update",
                column: "more_specific_sor_code_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_status_update_refined_appointment_window_id",
                table: "job_status_update",
                column: "refined_appointment_window_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_status_update_related_work_order_id",
                table: "job_status_update",
                column: "related_work_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_status_update_work_order_complete_id",
                table: "job_status_update",
                column: "work_order_complete_id");

            migrationBuilder.CreateIndex(
                name: "ix_operative_person_id",
                table: "operative",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_operative_work_order_complete_id",
                table: "operative",
                column: "work_order_complete_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_identification_id",
                table: "person",
                column: "identification_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_job_status_update_id",
                table: "person",
                column: "job_status_update_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_name_id",
                table: "person",
                column: "name_id");

            migrationBuilder.CreateIndex(
                name: "ix_score_score_set_id",
                table: "score",
                column: "score_set_id");

            migrationBuilder.CreateIndex(
                name: "ix_score_set_customer_satisfaction_id",
                table: "score_set",
                column: "customer_satisfaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_order_completes_work_order_id",
                table: "work_order_completes",
                column: "work_order_id");

            migrationBuilder.AddForeignKey(
                name: "fk_rate_schedule_items_work_order_completes_work_order_complet",
                table: "rate_schedule_items",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_trade_operative_operative_id",
                table: "trade",
                column: "operative_id",
                principalTable: "operative",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_elements_job_status_update_job_status_update_id",
                table: "work_elements",
                column: "job_status_update_id",
                principalTable: "job_status_update",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_elements_operative_operative_id",
                table: "work_elements",
                column: "operative_id",
                principalTable: "operative",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_elements_work_order_completes_work_order_complete_id",
                table: "work_elements",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_orders_work_order_completes_work_order_complete_id",
                table: "work_orders",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rate_schedule_items_work_order_completes_work_order_complet",
                table: "rate_schedule_items");

            migrationBuilder.DropForeignKey(
                name: "fk_trade_operative_operative_id",
                table: "trade");

            migrationBuilder.DropForeignKey(
                name: "fk_work_elements_job_status_update_job_status_update_id",
                table: "work_elements");

            migrationBuilder.DropForeignKey(
                name: "fk_work_elements_operative_operative_id",
                table: "work_elements");

            migrationBuilder.DropForeignKey(
                name: "fk_work_elements_work_order_completes_work_order_complete_id",
                table: "work_elements");

            migrationBuilder.DropForeignKey(
                name: "fk_work_orders_work_order_completes_work_order_complete_id",
                table: "work_orders");

            migrationBuilder.DropTable(
                name: "categorization");

            migrationBuilder.DropTable(
                name: "operative");

            migrationBuilder.DropTable(
                name: "score");

            migrationBuilder.DropTable(
                name: "person");

            migrationBuilder.DropTable(
                name: "score_set");

            migrationBuilder.DropTable(
                name: "identification");

            migrationBuilder.DropTable(
                name: "job_status_update");

            migrationBuilder.DropTable(
                name: "person_name");

            migrationBuilder.DropTable(
                name: "additional_work");

            migrationBuilder.DropTable(
                name: "communication");

            migrationBuilder.DropTable(
                name: "customer_satisfaction");

            migrationBuilder.DropTable(
                name: "appointment");

            migrationBuilder.DropTable(
                name: "work_order_completes");

            migrationBuilder.DropTable(
                name: "communication_channel");

            migrationBuilder.DropTable(
                name: "party");

            migrationBuilder.DropTable(
                name: "appointment_time_of_day");

            migrationBuilder.DropIndex(
                name: "ix_work_orders_work_order_complete_id",
                table: "work_orders");

            migrationBuilder.DropIndex(
                name: "ix_work_elements_job_status_update_id",
                table: "work_elements");

            migrationBuilder.DropIndex(
                name: "ix_work_elements_operative_id",
                table: "work_elements");

            migrationBuilder.DropIndex(
                name: "ix_work_elements_work_order_complete_id",
                table: "work_elements");

            migrationBuilder.DropIndex(
                name: "ix_trade_operative_id",
                table: "trade");

            migrationBuilder.DropIndex(
                name: "ix_rate_schedule_items_work_order_complete_id",
                table: "rate_schedule_items");

            migrationBuilder.DropColumn(
                name: "work_order_complete_id",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "job_status_update_id",
                table: "work_elements");

            migrationBuilder.DropColumn(
                name: "operative_id",
                table: "work_elements");

            migrationBuilder.DropColumn(
                name: "work_order_complete_id",
                table: "work_elements");

            migrationBuilder.DropColumn(
                name: "operative_id",
                table: "trade");

            migrationBuilder.DropColumn(
                name: "work_order_complete_id",
                table: "rate_schedule_items");
        }
    }
}
