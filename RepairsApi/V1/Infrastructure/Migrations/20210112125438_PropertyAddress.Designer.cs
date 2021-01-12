﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RepairsApi.V1.Infrastructure;

namespace RepairsApi.V1.Infrastructure.Migrations
{
    [DbContext(typeof(RepairsContext))]
    [Migration("20210112125438_PropertyAddress")]
    partial class PropertyAddress
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AddressLine")
                        .HasColumnName("address_line")
                        .HasColumnType("text");

                    b.Property<string>("BuildingName")
                        .HasColumnName("building_name")
                        .HasColumnType("text");

                    b.Property<string>("BuildingNumber")
                        .HasColumnName("building_number")
                        .HasColumnType("text");

                    b.Property<string>("CityName")
                        .HasColumnName("city_name")
                        .HasColumnType("text");

                    b.Property<string>("ComplexName")
                        .HasColumnName("complex_name")
                        .HasColumnType("text");

                    b.Property<int?>("Country")
                        .HasColumnName("country")
                        .HasColumnType("integer");

                    b.Property<string>("Department")
                        .HasColumnName("department")
                        .HasColumnType("text");

                    b.Property<string>("Floor")
                        .HasColumnName("floor")
                        .HasColumnType("text");

                    b.Property<string>("Plot")
                        .HasColumnName("plot")
                        .HasColumnType("text");

                    b.Property<string>("PostalCode")
                        .HasColumnName("postal_code")
                        .HasColumnType("text");

                    b.Property<string>("Postbox")
                        .HasColumnName("postbox")
                        .HasColumnType("text");

                    b.Property<string>("Room")
                        .HasColumnName("room")
                        .HasColumnType("text");

                    b.Property<string>("StreetName")
                        .HasColumnName("street_name")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .HasColumnName("type")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_address");

                    b.ToTable("address");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.AlertRegardingLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Comments")
                        .HasColumnName("comments")
                        .HasColumnType("text");

                    b.Property<int?>("Type")
                        .HasColumnName("type")
                        .HasColumnType("integer");

                    b.Property<int?>("WorkOrderId")
                        .HasColumnName("work_order_id")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("pk_location_alerts");

                    b.HasIndex("WorkOrderId")
                        .HasName("ix_location_alerts_work_order_id");

                    b.ToTable("location_alerts");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.AlertRegardingPerson", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Comments")
                        .HasColumnName("comments")
                        .HasColumnType("text");

                    b.Property<int?>("Type")
                        .HasColumnName("type")
                        .HasColumnType("integer");

                    b.Property<int?>("WorkOrderId")
                        .HasColumnName("work_order_id")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("pk_person_alerts");

                    b.HasIndex("WorkOrderId")
                        .HasName("ix_person_alerts_work_order_id");

                    b.ToTable("person_alerts");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.Dependency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTimeOffset?>("Duration")
                        .HasColumnName("duration")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("Type")
                        .HasColumnName("type")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("pk_dependency");

                    b.ToTable("dependency");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.PostalAddress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AddressId")
                        .HasColumnName("address_id")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("pk_postal_address");

                    b.HasIndex("AddressId")
                        .HasName("ix_postal_address_address_id");

                    b.ToTable("postal_address");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.PropertyAddress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AddressId")
                        .HasColumnName("address_id")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("pk_property_address");

                    b.HasIndex("AddressId")
                        .HasName("ix_property_address_address_id");

                    b.ToTable("property_address");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.PropertyClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AddressId")
                        .HasColumnName("address_id")
                        .HasColumnType("integer");

                    b.Property<int?>("SiteId")
                        .HasColumnName("site_id")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("pk_property_class");

                    b.HasIndex("AddressId")
                        .HasName("ix_property_class_address_id");

                    b.HasIndex("SiteId")
                        .HasName("ix_property_class_site_id");

                    b.ToTable("property_class");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.RateScheduleItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<string>("CustomCode")
                        .HasColumnName("custom_code")
                        .HasColumnType("text");

                    b.Property<string>("CustomName")
                        .HasColumnName("custom_name")
                        .HasColumnType("text");

                    b.Property<string>("M3NHFSORCode")
                        .HasColumnName("m3nhfsor_code")
                        .HasColumnType("text");

                    b.Property<Guid?>("WorkElementId")
                        .HasColumnName("work_element_id")
                        .HasColumnType("uuid");

                    b.HasKey("Id")
                        .HasName("pk_rate_schedule_items");

                    b.HasIndex("WorkElementId")
                        .HasName("ix_rate_schedule_items_work_element_id");

                    b.ToTable("rate_schedule_items");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.Site", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.HasKey("Id")
                        .HasName("pk_site");

                    b.ToTable("site");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.Trade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("Code")
                        .HasColumnName("code")
                        .HasColumnType("integer");

                    b.Property<string>("CustomCode")
                        .HasColumnName("custom_code")
                        .HasColumnType("text");

                    b.Property<string>("CustomName")
                        .HasColumnName("custom_name")
                        .HasColumnType("text");

                    b.Property<Guid?>("WorkElementId")
                        .HasColumnName("work_element_id")
                        .HasColumnType("uuid");

                    b.HasKey("Id")
                        .HasName("pk_trade");

                    b.HasIndex("WorkElementId")
                        .HasName("ix_trade_work_element_id");

                    b.ToTable("trade");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AddressId")
                        .HasColumnName("address_id")
                        .HasColumnType("integer");

                    b.Property<int?>("PropertyClassId")
                        .HasColumnName("property_class_id")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("pk_unit");

                    b.HasIndex("AddressId")
                        .HasName("ix_unit_address_id");

                    b.HasIndex("PropertyClassId")
                        .HasName("ix_unit_property_class_id");

                    b.ToTable("unit");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.WorkClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("WorkClassCode")
                        .HasColumnName("work_class_code")
                        .HasColumnType("integer");

                    b.Property<string>("WorkClassDescription")
                        .HasColumnName("work_class_description")
                        .HasColumnType("text");

                    b.Property<int?>("WorkClassSubTypeId")
                        .HasColumnName("work_class_sub_type_id")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("pk_work_class");

                    b.HasIndex("WorkClassSubTypeId")
                        .HasName("ix_work_class_work_class_sub_type_id");

                    b.ToTable("work_class");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.WorkClassSubType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("WorkClassSubTypeDescription")
                        .HasColumnName("work_class_sub_type_description")
                        .HasColumnType("text");

                    b.Property<string>("WorkClassSubTypeName")
                        .HasColumnName("work_class_sub_type_name")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_work_class_sub_type");

                    b.ToTable("work_class_sub_type");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.WorkElement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<bool?>("ContainsCapitalWork")
                        .HasColumnName("contains_capital_work")
                        .HasColumnType("boolean");

                    b.Property<int?>("ServiceChargeSubject")
                        .HasColumnName("service_charge_subject")
                        .HasColumnType("integer");

                    b.Property<int?>("WorkOrderId")
                        .HasColumnName("work_order_id")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("pk_work_elements");

                    b.HasIndex("WorkOrderId")
                        .HasName("ix_work_elements_work_order_id");

                    b.ToTable("work_elements");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.WorkElementDependency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("DependencyId")
                        .HasColumnName("dependency_id")
                        .HasColumnType("integer");

                    b.Property<Guid?>("DependsOnWorkElementId")
                        .HasColumnName("depends_on_work_element_id")
                        .HasColumnType("uuid");

                    b.HasKey("Id")
                        .HasName("pk_work_element_dependency");

                    b.HasIndex("DependencyId")
                        .HasName("ix_work_element_dependency_dependency_id");

                    b.HasIndex("DependsOnWorkElementId")
                        .HasName("ix_work_element_dependency_depends_on_work_element_id");

                    b.ToTable("work_element_dependency");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.WorkOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:IdentitySequenceOptions", "'10000000', '1', '', '', 'False', '1'")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AccessInformationId")
                        .HasColumnName("access_information_id")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("DateReported")
                        .HasColumnName("date_reported")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("DescriptionOfWork")
                        .HasColumnName("description_of_work")
                        .HasColumnType("text");

                    b.Property<double?>("EstimatedLaborHours")
                        .HasColumnName("estimated_labor_hours")
                        .HasColumnType("double precision");

                    b.Property<string>("LocationOfRepair")
                        .HasColumnName("location_of_repair")
                        .HasColumnType("text");

                    b.Property<string>("ParkingArrangements")
                        .HasColumnName("parking_arrangements")
                        .HasColumnType("text");

                    b.Property<int?>("SiteId")
                        .HasColumnName("site_id")
                        .HasColumnType("integer");

                    b.Property<int?>("WorkClassId")
                        .HasColumnName("work_class_id")
                        .HasColumnType("integer");

                    b.Property<Guid?>("WorkPriorityId")
                        .HasColumnName("work_priority_id")
                        .HasColumnType("uuid");

                    b.Property<int?>("WorkType")
                        .HasColumnName("work_type")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("pk_work_orders");

                    b.HasIndex("AccessInformationId")
                        .HasName("ix_work_orders_access_information_id");

                    b.HasIndex("SiteId")
                        .HasName("ix_work_orders_site_id");

                    b.HasIndex("WorkClassId")
                        .HasName("ix_work_orders_work_class_id");

                    b.HasIndex("WorkPriorityId")
                        .HasName("ix_work_orders_work_priority_id");

                    b.ToTable("work_orders");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.WorkOrderAccessInformation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_work_order_access_information");

                    b.ToTable("work_order_access_information");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.WorkPriority", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<int?>("PriorityCode")
                        .HasColumnName("priority_code")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("RequiredCompletionDateTime")
                        .HasColumnName("required_completion_date_time")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id")
                        .HasName("pk_work_priorities");

                    b.ToTable("work_priorities");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.AlertRegardingLocation", b =>
                {
                    b.HasOne("RepairsApi.V1.Infrastructure.WorkOrder", null)
                        .WithMany("LocationAlert")
                        .HasForeignKey("WorkOrderId")
                        .HasConstraintName("fk_location_alerts_work_orders_work_order_id");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.AlertRegardingPerson", b =>
                {
                    b.HasOne("RepairsApi.V1.Infrastructure.WorkOrder", null)
                        .WithMany("PersonAlert")
                        .HasForeignKey("WorkOrderId")
                        .HasConstraintName("fk_person_alerts_work_orders_work_order_id");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.PostalAddress", b =>
                {
                    b.HasOne("RepairsApi.V1.Infrastructure.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .HasConstraintName("fk_postal_address_address_address_id");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.PropertyAddress", b =>
                {
                    b.HasOne("RepairsApi.V1.Infrastructure.PostalAddress", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .HasConstraintName("fk_property_address_postal_address_address_id");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.PropertyClass", b =>
                {
                    b.HasOne("RepairsApi.V1.Infrastructure.PropertyAddress", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .HasConstraintName("fk_property_class_property_address_address_id");

                    b.HasOne("RepairsApi.V1.Infrastructure.Site", null)
                        .WithMany("PropertyClass")
                        .HasForeignKey("SiteId")
                        .HasConstraintName("fk_property_class_site_site_id");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.RateScheduleItem", b =>
                {
                    b.HasOne("RepairsApi.V1.Infrastructure.WorkElement", null)
                        .WithMany("RateScheduleItem")
                        .HasForeignKey("WorkElementId")
                        .HasConstraintName("fk_rate_schedule_items_work_elements_work_element_id");

                    b.OwnsOne("RepairsApi.V1.Infrastructure.Quantity", "Quantity", b1 =>
                        {
                            b1.Property<Guid>("RateScheduleItemId")
                                .HasColumnName("id")
                                .HasColumnType("uuid");

                            b1.Property<double>("Amount")
                                .HasColumnName("amount")
                                .HasColumnType("double precision");

                            b1.Property<int?>("UnitOfMeasurementCode")
                                .HasColumnName("unit_of_measurement_code")
                                .HasColumnType("integer");

                            b1.HasKey("RateScheduleItemId")
                                .HasName("pk_rate_schedule_items");

                            b1.ToTable("rate_schedule_items");

                            b1.WithOwner()
                                .HasForeignKey("RateScheduleItemId")
                                .HasConstraintName("fk_quantity_rate_schedule_items_rate_schedule_item_id");
                        });
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.Trade", b =>
                {
                    b.HasOne("RepairsApi.V1.Infrastructure.WorkElement", null)
                        .WithMany("Trade")
                        .HasForeignKey("WorkElementId")
                        .HasConstraintName("fk_trade_work_elements_work_element_id");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.Unit", b =>
                {
                    b.HasOne("RepairsApi.V1.Infrastructure.PropertyAddress", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .HasConstraintName("fk_unit_property_address_address_id");

                    b.HasOne("RepairsApi.V1.Infrastructure.PropertyClass", null)
                        .WithMany("Unit")
                        .HasForeignKey("PropertyClassId")
                        .HasConstraintName("fk_unit_property_class_property_class_id");

                    b.OwnsOne("RepairsApi.V1.Infrastructure.KeySafe", "KeySafe", b1 =>
                        {
                            b1.Property<int>("UnitId")
                                .HasColumnName("id")
                                .HasColumnType("integer");

                            b1.Property<string>("Code")
                                .HasColumnName("key_safe_code")
                                .HasColumnType("text");

                            b1.Property<string>("Location")
                                .HasColumnName("key_safe_location")
                                .HasColumnType("text");

                            b1.HasKey("UnitId")
                                .HasName("pk_unit");

                            b1.ToTable("unit");

                            b1.WithOwner()
                                .HasForeignKey("UnitId")
                                .HasConstraintName("fk_unit_unit_id");
                        });
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.WorkClass", b =>
                {
                    b.HasOne("RepairsApi.V1.Infrastructure.WorkClassSubType", "WorkClassSubType")
                        .WithMany()
                        .HasForeignKey("WorkClassSubTypeId")
                        .HasConstraintName("fk_work_class_work_class_sub_type_work_class_sub_type_id");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.WorkElement", b =>
                {
                    b.HasOne("RepairsApi.V1.Infrastructure.WorkOrder", null)
                        .WithMany("WorkElements")
                        .HasForeignKey("WorkOrderId")
                        .HasConstraintName("fk_work_elements_work_orders_work_order_id");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.WorkElementDependency", b =>
                {
                    b.HasOne("RepairsApi.V1.Infrastructure.Dependency", "Dependency")
                        .WithMany()
                        .HasForeignKey("DependencyId")
                        .HasConstraintName("fk_work_element_dependency_dependency_dependency_id");

                    b.HasOne("RepairsApi.V1.Infrastructure.WorkElement", "DependsOnWorkElement")
                        .WithMany("DependsOn")
                        .HasForeignKey("DependsOnWorkElementId")
                        .HasConstraintName("fk_work_element_dependency_work_elements_depends_on_work_eleme");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.WorkOrder", b =>
                {
                    b.HasOne("RepairsApi.V1.Infrastructure.WorkOrderAccessInformation", "AccessInformation")
                        .WithMany()
                        .HasForeignKey("AccessInformationId")
                        .HasConstraintName("fk_work_orders_work_order_access_information_access_informatio");

                    b.HasOne("RepairsApi.V1.Infrastructure.Site", "Site")
                        .WithMany()
                        .HasForeignKey("SiteId")
                        .HasConstraintName("fk_work_orders_site_site_id");

                    b.HasOne("RepairsApi.V1.Infrastructure.WorkClass", "WorkClass")
                        .WithMany()
                        .HasForeignKey("WorkClassId")
                        .HasConstraintName("fk_work_orders_work_class_work_class_id");

                    b.HasOne("RepairsApi.V1.Infrastructure.WorkPriority", "WorkPriority")
                        .WithMany()
                        .HasForeignKey("WorkPriorityId")
                        .HasConstraintName("fk_work_orders_work_priorities_work_priority_id");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.WorkOrderAccessInformation", b =>
                {
                    b.OwnsOne("RepairsApi.V1.Infrastructure.KeySafe", "Keysafe", b1 =>
                        {
                            b1.Property<int>("WorkOrderAccessInformationId")
                                .HasColumnName("id")
                                .HasColumnType("integer");

                            b1.Property<string>("Code")
                                .HasColumnName("keysafe_code")
                                .HasColumnType("text");

                            b1.Property<string>("Location")
                                .HasColumnName("keysafe_location")
                                .HasColumnType("text");

                            b1.HasKey("WorkOrderAccessInformationId")
                                .HasName("pk_work_order_access_information");

                            b1.ToTable("work_order_access_information");

                            b1.WithOwner()
                                .HasForeignKey("WorkOrderAccessInformationId")
                                .HasConstraintName("fk_work_order_access_information_work_order_access_information");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
