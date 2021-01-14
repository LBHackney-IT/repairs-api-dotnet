using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    public partial class OwnedEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_appointment_appointment_time_of_day_time_of_day_id",
                table: "appointment");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_update_additional_work_additional_work_id",
                table: "job_status_update");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_update_appointment_refined_appointment_window_id",
                table: "job_status_update");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_update_communication_customer_communication_chan",
                table: "job_status_update");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_update_customer_satisfaction_customer_feedback_id",
                table: "job_status_update");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_update_work_elements_more_specific_sor_code_id",
                table: "job_status_update");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_update_work_order_completes_work_order_complete_",
                table: "job_status_update");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_update_work_orders_related_work_order_id",
                table: "job_status_update");

            migrationBuilder.DropForeignKey(
                name: "fk_location_alerts_work_orders_work_order_id",
                table: "location_alerts");

            migrationBuilder.DropForeignKey(
                name: "fk_person_identification_identification_id",
                table: "person");

            migrationBuilder.DropForeignKey(
                name: "fk_person_job_status_update_job_status_update_id",
                table: "person");

            migrationBuilder.DropForeignKey(
                name: "fk_person_person_name_name_id",
                table: "person");

            migrationBuilder.DropForeignKey(
                name: "fk_person_alerts_work_orders_work_order_id",
                table: "person_alerts");

            migrationBuilder.DropForeignKey(
                name: "fk_rate_schedule_items_work_elements_work_element_id",
                table: "rate_schedule_items");

            migrationBuilder.DropForeignKey(
                name: "fk_rate_schedule_items_work_order_completes_work_order_complet",
                table: "rate_schedule_items");

            migrationBuilder.DropForeignKey(
                name: "fk_trade_work_elements_work_element_id",
                table: "trade");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_dependency_dependency_dependency_id",
                table: "work_element_dependency");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_dependency_work_elements_depends_on_work_eleme",
                table: "work_element_dependency");

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
                name: "fk_work_elements_work_orders_work_order_id",
                table: "work_elements");

            migrationBuilder.DropForeignKey(
                name: "fk_work_orders_work_class_work_class_id",
                table: "work_orders");

            migrationBuilder.DropForeignKey(
                name: "fk_work_orders_work_order_access_information_access_informatio",
                table: "work_orders");

            migrationBuilder.DropForeignKey(
                name: "fk_work_orders_work_priorities_work_priority_id",
                table: "work_orders");

            migrationBuilder.DropTable(
                name: "additional_work");

            migrationBuilder.DropTable(
                name: "appointment_time_of_day");

            migrationBuilder.DropTable(
                name: "communication");

            migrationBuilder.DropTable(
                name: "dependency");

            migrationBuilder.DropTable(
                name: "identification");

            migrationBuilder.DropTable(
                name: "person_name");

            migrationBuilder.DropTable(
                name: "work_class");

            migrationBuilder.DropTable(
                name: "work_order_access_information");

            migrationBuilder.DropTable(
                name: "work_priorities");

            migrationBuilder.DropTable(
                name: "communication_channel");

            migrationBuilder.DropTable(
                name: "work_class_sub_type");

            migrationBuilder.DropIndex(
                name: "ix_work_orders_access_information_id",
                table: "work_orders");

            migrationBuilder.DropIndex(
                name: "ix_work_orders_work_class_id",
                table: "work_orders");

            migrationBuilder.DropIndex(
                name: "ix_work_orders_work_priority_id",
                table: "work_orders");

            migrationBuilder.DropIndex(
                name: "ix_work_element_dependency_dependency_id",
                table: "work_element_dependency");

            migrationBuilder.DropIndex(
                name: "ix_person_identification_id",
                table: "person");

            migrationBuilder.DropIndex(
                name: "ix_person_name_id",
                table: "person");

            migrationBuilder.DropIndex(
                name: "ix_appointment_time_of_day_id",
                table: "appointment");

            migrationBuilder.DropPrimaryKey(
                name: "pk_work_elements",
                table: "work_elements");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rate_schedule_items",
                table: "rate_schedule_items");

            migrationBuilder.DropPrimaryKey(
                name: "pk_person_alerts",
                table: "person_alerts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_location_alerts",
                table: "location_alerts");

            migrationBuilder.DropPrimaryKey(
                name: "pk_job_status_update",
                table: "job_status_update");

            migrationBuilder.DropIndex(
                name: "ix_job_status_update_additional_work_id",
                table: "job_status_update");

            migrationBuilder.DropIndex(
                name: "ix_job_status_update_customer_communication_channel_attempted_",
                table: "job_status_update");

            migrationBuilder.DropColumn(
                name: "work_priority_id",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "identification_id",
                table: "person");

            migrationBuilder.DropColumn(
                name: "name_id",
                table: "person");

            migrationBuilder.DropColumn(
                name: "time_of_day_id",
                table: "appointment");

            migrationBuilder.RenameTable(
                name: "work_elements",
                newName: "work_element");

            migrationBuilder.RenameTable(
                name: "rate_schedule_items",
                newName: "rate_schedule_item");

            migrationBuilder.RenameTable(
                name: "person_alerts",
                newName: "alert_regarding_person");

            migrationBuilder.RenameTable(
                name: "location_alerts",
                newName: "alert_regarding_location");

            migrationBuilder.RenameTable(
                name: "job_status_update",
                newName: "job_status_updates");

            migrationBuilder.RenameColumn(
                name: "work_class_id",
                table: "work_orders",
                newName: "work_priority_priority_code");

            migrationBuilder.RenameColumn(
                name: "access_information_id",
                table: "work_orders",
                newName: "work_class_work_class_code");

            migrationBuilder.RenameColumn(
                name: "dependency_id",
                table: "work_element_dependency",
                newName: "dependency_type");

            migrationBuilder.RenameIndex(
                name: "ix_work_elements_work_order_id",
                table: "work_element",
                newName: "ix_work_element_work_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_elements_work_order_complete_id",
                table: "work_element",
                newName: "ix_work_element_work_order_complete_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_elements_operative_id",
                table: "work_element",
                newName: "ix_work_element_operative_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_elements_job_status_update_id",
                table: "work_element",
                newName: "ix_work_element_job_status_update_id");

            migrationBuilder.RenameColumn(
                name: "unit_of_measurement_code",
                table: "rate_schedule_item",
                newName: "quantity_unit_of_measurement_code");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "rate_schedule_item",
                newName: "quantity_amount");

            migrationBuilder.RenameIndex(
                name: "ix_rate_schedule_items_work_order_complete_id",
                table: "rate_schedule_item",
                newName: "ix_rate_schedule_item_work_order_complete_id");

            migrationBuilder.RenameIndex(
                name: "ix_rate_schedule_items_work_element_id",
                table: "rate_schedule_item",
                newName: "ix_rate_schedule_item_work_element_id");

            migrationBuilder.RenameIndex(
                name: "ix_person_alerts_work_order_id",
                table: "alert_regarding_person",
                newName: "ix_alert_regarding_person_work_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_location_alerts_work_order_id",
                table: "alert_regarding_location",
                newName: "ix_alert_regarding_location_work_order_id");

            migrationBuilder.RenameColumn(
                name: "customer_communication_channel_attempted_id",
                table: "job_status_updates",
                newName: "customer_communication_channel_attempted_channel_medium");

            migrationBuilder.RenameColumn(
                name: "additional_work_id",
                table: "job_status_updates",
                newName: "customer_communication_channel_attempted_channel_code");

            migrationBuilder.RenameIndex(
                name: "ix_job_status_update_work_order_complete_id",
                table: "job_status_updates",
                newName: "ix_job_status_updates_work_order_complete_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_status_update_related_work_order_id",
                table: "job_status_updates",
                newName: "ix_job_status_updates_related_work_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_status_update_refined_appointment_window_id",
                table: "job_status_updates",
                newName: "ix_job_status_updates_refined_appointment_window_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_status_update_more_specific_sor_code_id",
                table: "job_status_updates",
                newName: "ix_job_status_updates_more_specific_sor_code_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_status_update_customer_feedback_id",
                table: "job_status_updates",
                newName: "ix_job_status_updates_customer_feedback_id");

            migrationBuilder.AddColumn<string>(
                name: "access_information_description",
                table: "work_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "access_information_keysafe_code",
                table: "work_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "access_information_keysafe_location",
                table: "work_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "work_class_work_class_description",
                table: "work_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "work_class_work_class_sub_type_work_class_sub_type_description",
                table: "work_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "work_class_work_class_sub_type_work_class_sub_type_name",
                table: "work_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "work_priority_comments",
                table: "work_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "work_priority_number_of_days",
                table: "work_orders",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "work_priority_priority_description",
                table: "work_orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "work_priority_required_completion_date_time",
                table: "work_orders",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "dependency_duration",
                table: "work_element_dependency",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "identification_number",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "identification_type",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name_full",
                table: "person",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "time_of_day_earliest_arrival_time",
                table: "appointment",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "time_of_day_latest_arrival_time",
                table: "appointment",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "time_of_day_latest_completion_time",
                table: "appointment",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "time_of_day_name",
                table: "appointment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "additional_work_additional_work_order_id",
                table: "job_status_updates",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "additional_work_reason_required",
                table: "job_status_updates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "customer_communication_channel_attempted_value",
                table: "job_status_updates",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_work_element",
                table: "work_element",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rate_schedule_item",
                table: "rate_schedule_item",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_alert_regarding_person",
                table: "alert_regarding_person",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_alert_regarding_location",
                table: "alert_regarding_location",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_job_status_updates",
                table: "job_status_updates",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_job_status_updates_additional_work_additional_work_order_id",
                table: "job_status_updates",
                column: "additional_work_additional_work_order_id");

            migrationBuilder.AddForeignKey(
                name: "fk_alert_regarding_location_work_orders_work_order_id",
                table: "alert_regarding_location",
                column: "work_order_id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_alert_regarding_person_work_orders_work_order_id",
                table: "alert_regarding_person",
                column: "work_order_id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_updates_appointment_refined_appointment_window_id",
                table: "job_status_updates",
                column: "refined_appointment_window_id",
                principalTable: "appointment",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_updates_customer_satisfaction_customer_feedback_",
                table: "job_status_updates",
                column: "customer_feedback_id",
                principalTable: "customer_satisfaction",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_updates_work_element_more_specific_sor_code_id",
                table: "job_status_updates",
                column: "more_specific_sor_code_id",
                principalTable: "work_element",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_updates_work_order_completes_work_order_complete",
                table: "job_status_updates",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_updates_work_orders_additional_work_additional_w",
                table: "job_status_updates",
                column: "additional_work_additional_work_order_id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_updates_work_orders_related_work_order_id",
                table: "job_status_updates",
                column: "related_work_order_id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_person_job_status_updates_job_status_update_id",
                table: "person",
                column: "job_status_update_id",
                principalTable: "job_status_updates",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rate_schedule_item_work_element_work_element_id",
                table: "rate_schedule_item",
                column: "work_element_id",
                principalTable: "work_element",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rate_schedule_item_work_order_completes_work_order_complete",
                table: "rate_schedule_item",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_trade_work_element_work_element_id",
                table: "trade",
                column: "work_element_id",
                principalTable: "work_element",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_job_status_updates_job_status_update_id",
                table: "work_element",
                column: "job_status_update_id",
                principalTable: "job_status_updates",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_operative_operative_id",
                table: "work_element",
                column: "operative_id",
                principalTable: "operative",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_work_order_completes_work_order_complete_id",
                table: "work_element",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_work_orders_work_order_id",
                table: "work_element",
                column: "work_order_id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_dependency_work_element_depends_on_work_elemen",
                table: "work_element_dependency",
                column: "depends_on_work_element_id",
                principalTable: "work_element",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_alert_regarding_location_work_orders_work_order_id",
                table: "alert_regarding_location");

            migrationBuilder.DropForeignKey(
                name: "fk_alert_regarding_person_work_orders_work_order_id",
                table: "alert_regarding_person");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_updates_appointment_refined_appointment_window_id",
                table: "job_status_updates");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_updates_customer_satisfaction_customer_feedback_",
                table: "job_status_updates");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_updates_work_element_more_specific_sor_code_id",
                table: "job_status_updates");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_updates_work_order_completes_work_order_complete",
                table: "job_status_updates");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_updates_work_orders_additional_work_additional_w",
                table: "job_status_updates");

            migrationBuilder.DropForeignKey(
                name: "fk_job_status_updates_work_orders_related_work_order_id",
                table: "job_status_updates");

            migrationBuilder.DropForeignKey(
                name: "fk_person_job_status_updates_job_status_update_id",
                table: "person");

            migrationBuilder.DropForeignKey(
                name: "fk_rate_schedule_item_work_element_work_element_id",
                table: "rate_schedule_item");

            migrationBuilder.DropForeignKey(
                name: "fk_rate_schedule_item_work_order_completes_work_order_complete",
                table: "rate_schedule_item");

            migrationBuilder.DropForeignKey(
                name: "fk_trade_work_element_work_element_id",
                table: "trade");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_job_status_updates_job_status_update_id",
                table: "work_element");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_operative_operative_id",
                table: "work_element");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_work_order_completes_work_order_complete_id",
                table: "work_element");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_work_orders_work_order_id",
                table: "work_element");

            migrationBuilder.DropForeignKey(
                name: "fk_work_element_dependency_work_element_depends_on_work_elemen",
                table: "work_element_dependency");

            migrationBuilder.DropPrimaryKey(
                name: "pk_work_element",
                table: "work_element");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rate_schedule_item",
                table: "rate_schedule_item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_job_status_updates",
                table: "job_status_updates");

            migrationBuilder.DropIndex(
                name: "ix_job_status_updates_additional_work_additional_work_order_id",
                table: "job_status_updates");

            migrationBuilder.DropPrimaryKey(
                name: "pk_alert_regarding_person",
                table: "alert_regarding_person");

            migrationBuilder.DropPrimaryKey(
                name: "pk_alert_regarding_location",
                table: "alert_regarding_location");

            migrationBuilder.DropColumn(
                name: "access_information_description",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "access_information_keysafe_code",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "access_information_keysafe_location",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "work_class_work_class_description",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "work_class_work_class_sub_type_work_class_sub_type_description",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "work_class_work_class_sub_type_work_class_sub_type_name",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "work_priority_comments",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "work_priority_number_of_days",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "work_priority_priority_description",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "work_priority_required_completion_date_time",
                table: "work_orders");

            migrationBuilder.DropColumn(
                name: "dependency_duration",
                table: "work_element_dependency");

            migrationBuilder.DropColumn(
                name: "identification_number",
                table: "person");

            migrationBuilder.DropColumn(
                name: "identification_type",
                table: "person");

            migrationBuilder.DropColumn(
                name: "name_full",
                table: "person");

            migrationBuilder.DropColumn(
                name: "time_of_day_earliest_arrival_time",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "time_of_day_latest_arrival_time",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "time_of_day_latest_completion_time",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "time_of_day_name",
                table: "appointment");

            migrationBuilder.DropColumn(
                name: "additional_work_additional_work_order_id",
                table: "job_status_updates");

            migrationBuilder.DropColumn(
                name: "additional_work_reason_required",
                table: "job_status_updates");

            migrationBuilder.DropColumn(
                name: "customer_communication_channel_attempted_value",
                table: "job_status_updates");

            migrationBuilder.RenameTable(
                name: "work_element",
                newName: "work_elements");

            migrationBuilder.RenameTable(
                name: "rate_schedule_item",
                newName: "rate_schedule_items");

            migrationBuilder.RenameTable(
                name: "job_status_updates",
                newName: "job_status_update");

            migrationBuilder.RenameTable(
                name: "alert_regarding_person",
                newName: "person_alerts");

            migrationBuilder.RenameTable(
                name: "alert_regarding_location",
                newName: "location_alerts");

            migrationBuilder.RenameColumn(
                name: "work_priority_priority_code",
                table: "work_orders",
                newName: "work_class_id");

            migrationBuilder.RenameColumn(
                name: "work_class_work_class_code",
                table: "work_orders",
                newName: "access_information_id");

            migrationBuilder.RenameColumn(
                name: "dependency_type",
                table: "work_element_dependency",
                newName: "dependency_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_element_work_order_id",
                table: "work_elements",
                newName: "ix_work_elements_work_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_element_work_order_complete_id",
                table: "work_elements",
                newName: "ix_work_elements_work_order_complete_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_element_operative_id",
                table: "work_elements",
                newName: "ix_work_elements_operative_id");

            migrationBuilder.RenameIndex(
                name: "ix_work_element_job_status_update_id",
                table: "work_elements",
                newName: "ix_work_elements_job_status_update_id");

            migrationBuilder.RenameColumn(
                name: "quantity_unit_of_measurement_code",
                table: "rate_schedule_items",
                newName: "unit_of_measurement_code");

            migrationBuilder.RenameColumn(
                name: "quantity_amount",
                table: "rate_schedule_items",
                newName: "amount");

            migrationBuilder.RenameIndex(
                name: "ix_rate_schedule_item_work_order_complete_id",
                table: "rate_schedule_items",
                newName: "ix_rate_schedule_items_work_order_complete_id");

            migrationBuilder.RenameIndex(
                name: "ix_rate_schedule_item_work_element_id",
                table: "rate_schedule_items",
                newName: "ix_rate_schedule_items_work_element_id");

            migrationBuilder.RenameColumn(
                name: "customer_communication_channel_attempted_channel_medium",
                table: "job_status_update",
                newName: "customer_communication_channel_attempted_id");

            migrationBuilder.RenameColumn(
                name: "customer_communication_channel_attempted_channel_code",
                table: "job_status_update",
                newName: "additional_work_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_status_updates_work_order_complete_id",
                table: "job_status_update",
                newName: "ix_job_status_update_work_order_complete_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_status_updates_related_work_order_id",
                table: "job_status_update",
                newName: "ix_job_status_update_related_work_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_status_updates_refined_appointment_window_id",
                table: "job_status_update",
                newName: "ix_job_status_update_refined_appointment_window_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_status_updates_more_specific_sor_code_id",
                table: "job_status_update",
                newName: "ix_job_status_update_more_specific_sor_code_id");

            migrationBuilder.RenameIndex(
                name: "ix_job_status_updates_customer_feedback_id",
                table: "job_status_update",
                newName: "ix_job_status_update_customer_feedback_id");

            migrationBuilder.RenameIndex(
                name: "ix_alert_regarding_person_work_order_id",
                table: "person_alerts",
                newName: "ix_person_alerts_work_order_id");

            migrationBuilder.RenameIndex(
                name: "ix_alert_regarding_location_work_order_id",
                table: "location_alerts",
                newName: "ix_location_alerts_work_order_id");

            migrationBuilder.AddColumn<Guid>(
                name: "work_priority_id",
                table: "work_orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "identification_id",
                table: "person",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "name_id",
                table: "person",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "time_of_day_id",
                table: "appointment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_work_elements",
                table: "work_elements",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rate_schedule_items",
                table: "rate_schedule_items",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_job_status_update",
                table: "job_status_update",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_person_alerts",
                table: "person_alerts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_location_alerts",
                table: "location_alerts",
                column: "id");

            migrationBuilder.CreateTable(
                name: "additional_work",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    additional_work_order_id = table.Column<int>(type: "integer", nullable: true),
                    reason_required = table.Column<string>(type: "text", nullable: true)
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
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    earliest_arrival_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    latest_arrival_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    latest_completion_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_appointment_time_of_day", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "communication_channel",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<int>(type: "integer", nullable: true),
                    medium = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_communication_channel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dependency",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    duration = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dependency", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "identification",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    number = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identification", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "person_name",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    full = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_name", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "work_class_sub_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    work_class_sub_type_description = table.Column<string>(type: "text", nullable: true),
                    work_class_sub_type_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_class_sub_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "work_order_access_information",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: true),
                    keysafe_code = table.Column<string>(type: "text", nullable: true),
                    keysafe_location = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_order_access_information", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "work_priorities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    comments = table.Column<string>(type: "text", nullable: true),
                    number_of_days = table.Column<double>(type: "double precision", nullable: true),
                    priority_code = table.Column<int>(type: "integer", nullable: true),
                    priority_description = table.Column<string>(type: "text", nullable: true),
                    required_completion_date_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_priorities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "communication",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    channel_id = table.Column<int>(type: "integer", nullable: true),
                    value = table.Column<string>(type: "text", nullable: true)
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
                name: "work_class",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    work_class_code = table.Column<int>(type: "integer", nullable: true),
                    work_class_description = table.Column<string>(type: "text", nullable: true),
                    work_class_sub_type_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_class", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_class_work_class_sub_type_work_class_sub_type_id",
                        column: x => x.work_class_sub_type_id,
                        principalTable: "work_class_sub_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_access_information_id",
                table: "work_orders",
                column: "access_information_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_work_class_id",
                table: "work_orders",
                column: "work_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_work_priority_id",
                table: "work_orders",
                column: "work_priority_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_element_dependency_dependency_id",
                table: "work_element_dependency",
                column: "dependency_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_identification_id",
                table: "person",
                column: "identification_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_name_id",
                table: "person",
                column: "name_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointment_time_of_day_id",
                table: "appointment",
                column: "time_of_day_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_status_update_additional_work_id",
                table: "job_status_update",
                column: "additional_work_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_status_update_customer_communication_channel_attempted_",
                table: "job_status_update",
                column: "customer_communication_channel_attempted_id");

            migrationBuilder.CreateIndex(
                name: "ix_additional_work_additional_work_order_id",
                table: "additional_work",
                column: "additional_work_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_communication_channel_id",
                table: "communication",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_class_work_class_sub_type_id",
                table: "work_class",
                column: "work_class_sub_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_appointment_appointment_time_of_day_time_of_day_id",
                table: "appointment",
                column: "time_of_day_id",
                principalTable: "appointment_time_of_day",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_update_additional_work_additional_work_id",
                table: "job_status_update",
                column: "additional_work_id",
                principalTable: "additional_work",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_update_appointment_refined_appointment_window_id",
                table: "job_status_update",
                column: "refined_appointment_window_id",
                principalTable: "appointment",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_update_communication_customer_communication_chan",
                table: "job_status_update",
                column: "customer_communication_channel_attempted_id",
                principalTable: "communication",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_update_customer_satisfaction_customer_feedback_id",
                table: "job_status_update",
                column: "customer_feedback_id",
                principalTable: "customer_satisfaction",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_update_work_elements_more_specific_sor_code_id",
                table: "job_status_update",
                column: "more_specific_sor_code_id",
                principalTable: "work_elements",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_update_work_order_completes_work_order_complete_",
                table: "job_status_update",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_job_status_update_work_orders_related_work_order_id",
                table: "job_status_update",
                column: "related_work_order_id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_location_alerts_work_orders_work_order_id",
                table: "location_alerts",
                column: "work_order_id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_person_identification_identification_id",
                table: "person",
                column: "identification_id",
                principalTable: "identification",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_person_job_status_update_job_status_update_id",
                table: "person",
                column: "job_status_update_id",
                principalTable: "job_status_update",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_person_person_name_name_id",
                table: "person",
                column: "name_id",
                principalTable: "person_name",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_person_alerts_work_orders_work_order_id",
                table: "person_alerts",
                column: "work_order_id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rate_schedule_items_work_elements_work_element_id",
                table: "rate_schedule_items",
                column: "work_element_id",
                principalTable: "work_elements",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_rate_schedule_items_work_order_completes_work_order_complet",
                table: "rate_schedule_items",
                column: "work_order_complete_id",
                principalTable: "work_order_completes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_trade_work_elements_work_element_id",
                table: "trade",
                column: "work_element_id",
                principalTable: "work_elements",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_dependency_dependency_dependency_id",
                table: "work_element_dependency",
                column: "dependency_id",
                principalTable: "dependency",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_element_dependency_work_elements_depends_on_work_eleme",
                table: "work_element_dependency",
                column: "depends_on_work_element_id",
                principalTable: "work_elements",
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
                name: "fk_work_elements_work_orders_work_order_id",
                table: "work_elements",
                column: "work_order_id",
                principalTable: "work_orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_orders_work_class_work_class_id",
                table: "work_orders",
                column: "work_class_id",
                principalTable: "work_class",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_orders_work_order_access_information_access_informatio",
                table: "work_orders",
                column: "access_information_id",
                principalTable: "work_order_access_information",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_work_orders_work_priorities_work_priority_id",
                table: "work_orders",
                column: "work_priority_id",
                principalTable: "work_priorities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
