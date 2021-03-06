﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RepairsApi.V2.Infrastructure;

namespace RepairsApi.V2.Infrastructure.Migrations
{
    [DbContext(typeof(RepairsContext))]
    [Migration("20201216152728_AddWorkPriority")]
    partial class AddWorkPriority
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("RepairsApi.V2.Infrastructure.RateScheduleItem", b =>
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

                    b.Property<Guid>("WorkElementId")
                        .HasColumnName("work_element_id")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("WorkElementId");

                    b.ToTable("rate_schedule_item");
                });

            modelBuilder.Entity("RepairsApi.V2.Infrastructure.WorkElement", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<bool>("ContainsCapitalWork")
                        .HasColumnName("contains_capital_work")
                        .HasColumnType("boolean");

                    b.Property<string>("ServiceChargeSubject")
                        .HasColumnName("service_charge_subject")
                        .HasColumnType("text");

                    b.Property<string>("trade")
                        .HasColumnName("trade ")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("work_element");
                });

            modelBuilder.Entity("RepairsApi.V2.Infrastructure.WorkPriority", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uuid");

                    b.Property<string>("Comments")
                        .HasColumnName("comments")
                        .HasColumnType("text");

                    b.Property<double>("NumberOfDays")
                        .HasColumnName("number_of_days")
                        .HasColumnType("double precision");

                    b.Property<int?>("PriorityCodeId")
                        .HasColumnType("integer");

                    b.Property<string>("PriorityDescription")
                        .HasColumnName("priority_description")
                        .HasColumnType("text");

                    b.Property<DateTime>("RequiredCompletionDateTime")
                        .HasColumnName("required_completion_datetime")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("PriorityCodeId");

                    b.ToTable("work_priority");
                });

            modelBuilder.Entity("RepairsApi.V2.Infrastructure.WorkPriorityCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("work_priority_code");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Emergency"
                        },
                        new
                        {
                            Id = 2,
                            Name = "High"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Medium"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Low"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Deferred"
                        });
                });

            modelBuilder.Entity("RepairsApi.V2.Infrastructure.RateScheduleItem", b =>
                {
                    b.HasOne("RepairsApi.V2.Infrastructure.WorkElement", "WorkElement")
                        .WithMany("RateScheduleItem")
                        .HasForeignKey("WorkElementId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("RepairsApi.V2.Infrastructure.Quantity", "Quantity", b1 =>
                        {
                            b1.Property<Guid>("RateScheduleItemId")
                                .HasColumnType("uuid");

                            b1.Property<int>("Amount")
                                .HasColumnName("amount")
                                .HasColumnType("integer");

                            b1.Property<string>("UnitOfMeasurementCode")
                                .HasColumnName("unit_of_measurement_code")
                                .HasColumnType("text");

                            b1.HasKey("RateScheduleItemId");

                            b1.ToTable("rate_schedule_item");

                            b1.WithOwner()
                                .HasForeignKey("RateScheduleItemId");
                        });
                });

            modelBuilder.Entity("RepairsApi.V2.Infrastructure.WorkPriority", b =>
                {
                    b.HasOne("RepairsApi.V2.Infrastructure.WorkPriorityCode", "PriorityCode")
                        .WithMany()
                        .HasForeignKey("PriorityCodeId");
                });
#pragma warning restore 612, 618
        }
    }
}
