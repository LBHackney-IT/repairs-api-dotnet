﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RepairsApi.V1.Infrastructure;

namespace RepairsApi.V1.Infrastructure.Migrations
{
    [DbContext(typeof(RepairsContext))]
    partial class RepairsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

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

                    b.HasKey("Id");

                    b.ToTable("rate_schedule_item");
                });

            modelBuilder.Entity("RepairsApi.V1.Infrastructure.RateScheduleItem", b =>
                {
                    b.OwnsOne("RepairsApi.V1.Infrastructure.Quantity", "Quantity", b1 =>
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
#pragma warning restore 612, 618
        }
    }
}
