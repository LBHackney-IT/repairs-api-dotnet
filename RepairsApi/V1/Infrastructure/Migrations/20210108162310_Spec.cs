using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RepairsApi.V1.Infrastructure.Migrations
{
    public partial class Spec : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rate_schedule_item_work_element_work_element_id",
                table: "rate_schedule_item");

            migrationBuilder.DropForeignKey(
                name: "FK_work_priority_work_priority_code_PriorityCodeId",
                table: "work_priority");

            migrationBuilder.DropPrimaryKey(
                name: "PK_work_priority_code",
                table: "work_priority_code");

            migrationBuilder.DropPrimaryKey(
                name: "PK_work_priority",
                table: "work_priority");

            migrationBuilder.DropPrimaryKey(
                name: "PK_work_element",
                table: "work_element");

            migrationBuilder.DropPrimaryKey(
                name: "PK_rate_schedule_item",
                table: "rate_schedule_item");

            migrationBuilder.DropColumn(
                name: "comments",
                table: "work_priority");

            migrationBuilder.DropColumn(
                name: "number_of_days",
                table: "work_priority");

            migrationBuilder.DropColumn(
                name: "priority_description",
                table: "work_priority");

            migrationBuilder.DropColumn(
                name: "trade ",
                table: "work_element");

            migrationBuilder.RenameTable(
                name: "work_priority_code",
                newName: "work_priority_codes");

            migrationBuilder.RenameTable(
                name: "work_priority",
                newName: "work_priorities");

            migrationBuilder.RenameTable(
                name: "work_element",
                newName: "work_elements");

            migrationBuilder.RenameTable(
                name: "rate_schedule_item",
                newName: "rate_schedule_items");

            migrationBuilder.RenameColumn(
                name: "required_completion_datetime",
                table: "work_priorities",
                newName: "required_completion_date_time");

            migrationBuilder.RenameColumn(
                name: "PriorityCodeId",
                table: "work_priorities",
                newName: "priority_code_id");

            migrationBuilder.RenameIndex(
                name: "IX_work_priority_PriorityCodeId",
                table: "work_priorities",
                newName: "ix_work_priorities_priority_code_id");

            migrationBuilder.RenameIndex(
                name: "IX_rate_schedule_item_work_element_id",
                table: "rate_schedule_items",
                newName: "ix_rate_schedule_items_work_element_id");

            migrationBuilder.AlterColumn<int>(
                name: "service_charge_subject",
                table: "work_elements",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "work_order_id",
                table: "work_elements",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "work_element_id",
                table: "rate_schedule_items",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "unit_of_measurement_code",
                table: "rate_schedule_items",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "rate_schedule_items",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "m3nhfsor_code",
                table: "rate_schedule_items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "pk_work_priority_codes",
                table: "work_priority_codes",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_work_priorities",
                table: "work_priorities",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_work_elements",
                table: "work_elements",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_rate_schedule_items",
                table: "rate_schedule_items",
                column: "id");

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    postbox = table.Column<string>(nullable: true),
                    room = table.Column<string>(nullable: true),
                    department = table.Column<string>(nullable: true),
                    floor = table.Column<string>(nullable: true),
                    plot = table.Column<string>(nullable: true),
                    building_number = table.Column<string>(nullable: true),
                    building_name = table.Column<string>(nullable: true),
                    complex_name = table.Column<string>(nullable: true),
                    street_name = table.Column<string>(nullable: true),
                    city_name = table.Column<string>(nullable: true),
                    country = table.Column<int>(nullable: false),
                    address_line = table.Column<string>(nullable: true),
                    type = table.Column<string>(nullable: true),
                    postal_code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_address", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dependency",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<int>(nullable: false),
                    duration = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dependency", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "site",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_site", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "trade",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<int>(nullable: false),
                    custom_code = table.Column<string>(nullable: true),
                    custom_name = table.Column<string>(nullable: true),
                    work_element_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trade", x => x.id);
                    table.ForeignKey(
                        name: "fk_trade_work_elements_work_element_id",
                        column: x => x.work_element_id,
                        principalTable: "work_elements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_class_sub_type",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    work_class_sub_type_name = table.Column<string>(nullable: true),
                    work_class_sub_type_description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_class_sub_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "work_order_access_information",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(nullable: true),
                    keysafe_location = table.Column<string>(nullable: true),
                    keysafe_code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_order_access_information", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "postal_address",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address_id = table.Column<int>(nullable: true)
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
                name: "work_element_dependency",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dependency_id = table.Column<int>(nullable: true),
                    depends_on_work_element_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_element_dependency", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_element_dependency_dependency_dependency_id",
                        column: x => x.dependency_id,
                        principalTable: "dependency",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_element_dependency_work_elements_depends_on_work_eleme",
                        column: x => x.depends_on_work_element_id,
                        principalTable: "work_elements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "property_class",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    site_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_property_class", x => x.id);
                    table.ForeignKey(
                        name: "fk_property_class_site_site_id",
                        column: x => x.site_id,
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_class",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    work_class_description = table.Column<string>(nullable: true),
                    work_class_code = table.Column<int>(nullable: false),
                    work_class_sub_type_id = table.Column<int>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "property_address",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_property_address", x => x.id);
                    table.ForeignKey(
                        name: "fk_property_address_postal_address_address_id",
                        column: x => x.address_id,
                        principalTable: "postal_address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_orders",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'10000000', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description_of_work = table.Column<string>(nullable: true),
                    estimated_labor_hours = table.Column<double>(nullable: false),
                    work_type = table.Column<int>(nullable: false),
                    parking_arrangements = table.Column<string>(nullable: true),
                    location_of_repair = table.Column<string>(nullable: true),
                    date_reported = table.Column<DateTime>(nullable: false),
                    work_class_id = table.Column<int>(nullable: true),
                    work_priority_id = table.Column<Guid>(nullable: true),
                    site_id = table.Column<int>(nullable: true),
                    access_information_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_orders_work_order_access_information_access_informatio",
                        column: x => x.access_information_id,
                        principalTable: "work_order_access_information",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_orders_site_site_id",
                        column: x => x.site_id,
                        principalTable: "site",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_orders_work_class_work_class_id",
                        column: x => x.work_class_id,
                        principalTable: "work_class",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_orders_work_priorities_work_priority_id",
                        column: x => x.work_priority_id,
                        principalTable: "work_priorities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "unit",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address_id = table.Column<int>(nullable: true),
                    key_safe_location = table.Column<string>(nullable: true),
                    key_safe_code = table.Column<string>(nullable: true),
                    property_class_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_unit", x => x.id);
                    table.ForeignKey(
                        name: "fk_unit_property_address_address_id",
                        column: x => x.address_id,
                        principalTable: "property_address",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_unit_property_class_property_class_id",
                        column: x => x.property_class_id,
                        principalTable: "property_class",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "location_alerts",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comments = table.Column<string>(nullable: true),
                    type = table.Column<int>(nullable: false),
                    work_order_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_location_alerts", x => x.id);
                    table.ForeignKey(
                        name: "fk_location_alerts_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "person_alerts",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comments = table.Column<string>(nullable: true),
                    type = table.Column<int>(nullable: false),
                    work_order_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_alerts", x => x.id);
                    table.ForeignKey(
                        name: "fk_person_alerts_work_orders_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "work_orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_work_elements_work_order_id",
                table: "work_elements",
                column: "work_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_location_alerts_work_order_id",
                table: "location_alerts",
                column: "work_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_alerts_work_order_id",
                table: "person_alerts",
                column: "work_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_postal_address_address_id",
                table: "postal_address",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "ix_property_address_address_id",
                table: "property_address",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "ix_property_class_site_id",
                table: "property_class",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_trade_work_element_id",
                table: "trade",
                column: "work_element_id");

            migrationBuilder.CreateIndex(
                name: "ix_unit_address_id",
                table: "unit",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "ix_unit_property_class_id",
                table: "unit",
                column: "property_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_class_work_class_sub_type_id",
                table: "work_class",
                column: "work_class_sub_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_element_dependency_dependency_id",
                table: "work_element_dependency",
                column: "dependency_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_element_dependency_depends_on_work_element_id",
                table: "work_element_dependency",
                column: "depends_on_work_element_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_access_information_id",
                table: "work_orders",
                column: "access_information_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_site_id",
                table: "work_orders",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_work_class_id",
                table: "work_orders",
                column: "work_class_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_work_priority_id",
                table: "work_orders",
                column: "work_priority_id");

            migrationBuilder.AddForeignKey(
                name: "fk_rate_schedule_items_work_elements_work_element_id",
                table: "rate_schedule_items",
                column: "work_element_id",
                principalTable: "work_elements",
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
                name: "fk_work_priorities_work_priority_codes_priority_code_id",
                table: "work_priorities",
                column: "priority_code_id",
                principalTable: "work_priority_codes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rate_schedule_items_work_elements_work_element_id",
                table: "rate_schedule_items");

            migrationBuilder.DropForeignKey(
                name: "fk_work_elements_work_orders_work_order_id",
                table: "work_elements");

            migrationBuilder.DropForeignKey(
                name: "fk_work_priorities_work_priority_codes_priority_code_id",
                table: "work_priorities");

            migrationBuilder.DropTable(
                name: "location_alerts");

            migrationBuilder.DropTable(
                name: "person_alerts");

            migrationBuilder.DropTable(
                name: "trade");

            migrationBuilder.DropTable(
                name: "unit");

            migrationBuilder.DropTable(
                name: "work_element_dependency");

            migrationBuilder.DropTable(
                name: "work_orders");

            migrationBuilder.DropTable(
                name: "property_address");

            migrationBuilder.DropTable(
                name: "property_class");

            migrationBuilder.DropTable(
                name: "dependency");

            migrationBuilder.DropTable(
                name: "work_order_access_information");

            migrationBuilder.DropTable(
                name: "work_class");

            migrationBuilder.DropTable(
                name: "postal_address");

            migrationBuilder.DropTable(
                name: "site");

            migrationBuilder.DropTable(
                name: "work_class_sub_type");

            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropPrimaryKey(
                name: "pk_work_priority_codes",
                table: "work_priority_codes");

            migrationBuilder.DropPrimaryKey(
                name: "pk_work_priorities",
                table: "work_priorities");

            migrationBuilder.DropPrimaryKey(
                name: "pk_work_elements",
                table: "work_elements");

            migrationBuilder.DropIndex(
                name: "ix_work_elements_work_order_id",
                table: "work_elements");

            migrationBuilder.DropPrimaryKey(
                name: "pk_rate_schedule_items",
                table: "rate_schedule_items");

            migrationBuilder.DropColumn(
                name: "work_order_id",
                table: "work_elements");

            migrationBuilder.DropColumn(
                name: "m3nhfsor_code",
                table: "rate_schedule_items");

            migrationBuilder.RenameTable(
                name: "work_priority_codes",
                newName: "work_priority_code");

            migrationBuilder.RenameTable(
                name: "work_priorities",
                newName: "work_priority");

            migrationBuilder.RenameTable(
                name: "work_elements",
                newName: "work_element");

            migrationBuilder.RenameTable(
                name: "rate_schedule_items",
                newName: "rate_schedule_item");

            migrationBuilder.RenameColumn(
                name: "required_completion_date_time",
                table: "work_priority",
                newName: "required_completion_datetime");

            migrationBuilder.RenameColumn(
                name: "priority_code_id",
                table: "work_priority",
                newName: "PriorityCodeId");

            migrationBuilder.RenameIndex(
                name: "ix_work_priorities_priority_code_id",
                table: "work_priority",
                newName: "IX_work_priority_PriorityCodeId");

            migrationBuilder.RenameIndex(
                name: "ix_rate_schedule_items_work_element_id",
                table: "rate_schedule_item",
                newName: "IX_rate_schedule_item_work_element_id");

            migrationBuilder.AddColumn<string>(
                name: "comments",
                table: "work_priority",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "number_of_days",
                table: "work_priority",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "priority_description",
                table: "work_priority",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "service_charge_subject",
                table: "work_element",
                type: "text",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "trade ",
                table: "work_element",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "work_element_id",
                table: "rate_schedule_item",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "unit_of_measurement_code",
                table: "rate_schedule_item",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "amount",
                table: "rate_schedule_item",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_work_priority_code",
                table: "work_priority_code",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_work_priority",
                table: "work_priority",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_work_element",
                table: "work_element",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rate_schedule_item",
                table: "rate_schedule_item",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_rate_schedule_item_work_element_work_element_id",
                table: "rate_schedule_item",
                column: "work_element_id",
                principalTable: "work_element",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_work_priority_work_priority_code_PriorityCodeId",
                table: "work_priority",
                column: "PriorityCodeId",
                principalTable: "work_priority_code",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
