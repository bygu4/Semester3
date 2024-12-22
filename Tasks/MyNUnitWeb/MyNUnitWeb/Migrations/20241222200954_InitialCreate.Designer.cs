﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyNUnitWeb.Data;

#nullable disable

namespace MyNUnitWeb.Migrations
{
    [DbContext(typeof(TestRunDbContext))]
    [Migration("20241222200954_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("MyNUnit.Core.TestResult", b =>
                {
                    b.Property<int>("TestResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AssemblyName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClassName")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("Elapsed")
                        .HasColumnType("TEXT");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("TEXT");

                    b.Property<string>("IgnoreReason")
                        .HasColumnType("TEXT");

                    b.Property<string>("MethodName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("TestSummaryId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TestResultId");

                    b.HasIndex("TestSummaryId");

                    b.ToTable("TestResult");
                });

            modelBuilder.Entity("MyNUnit.Core.TestSummary", b =>
                {
                    b.Property<int>("TestSummaryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("Elapsed")
                        .HasColumnType("TEXT");

                    b.Property<int>("NumberOfTestsFailed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NumberOfTestsIgnored")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NumberOfTestsPassed")
                        .HasColumnType("INTEGER");

                    b.HasKey("TestSummaryId");

                    b.ToTable("TestSummary");
                });

            modelBuilder.Entity("MyNUnitWeb.Data.TestRun", b =>
                {
                    b.Property<int>("TestRunId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("SummaryTestSummaryId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("TimeOfRun")
                        .HasColumnType("TEXT");

                    b.HasKey("TestRunId");

                    b.HasIndex("SummaryTestSummaryId");

                    b.ToTable("TestRuns");
                });

            modelBuilder.Entity("MyNUnit.Core.TestResult", b =>
                {
                    b.HasOne("MyNUnit.Core.TestSummary", null)
                        .WithMany("TestResults")
                        .HasForeignKey("TestSummaryId");
                });

            modelBuilder.Entity("MyNUnitWeb.Data.TestRun", b =>
                {
                    b.HasOne("MyNUnit.Core.TestSummary", "Summary")
                        .WithMany()
                        .HasForeignKey("SummaryTestSummaryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Summary");
                });

            modelBuilder.Entity("MyNUnit.Core.TestSummary", b =>
                {
                    b.Navigation("TestResults");
                });
#pragma warning restore 612, 618
        }
    }
}
