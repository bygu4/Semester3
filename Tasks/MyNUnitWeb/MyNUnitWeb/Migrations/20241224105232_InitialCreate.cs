﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNUnitWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestSummary",
                columns: table => new
                {
                    TestSummaryId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumberOfTestsPassed = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberOfTestsFailed = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberOfTestsIgnored = table.Column<int>(type: "INTEGER", nullable: false),
                    Elapsed = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSummary", x => x.TestSummaryId);
                });

            migrationBuilder.CreateTable(
                name: "TestResult",
                columns: table => new
                {
                    TestResultId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssemblyName = table.Column<string>(type: "TEXT", nullable: true),
                    ClassName = table.Column<string>(type: "TEXT", nullable: true),
                    MethodName = table.Column<string>(type: "TEXT", nullable: false),
                    IgnoreReason = table.Column<string>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    Elapsed = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TestSummaryId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResult", x => x.TestResultId);
                    table.ForeignKey(
                        name: "FK_TestResult_TestSummary_TestSummaryId",
                        column: x => x.TestSummaryId,
                        principalTable: "TestSummary",
                        principalColumn: "TestSummaryId");
                });

            migrationBuilder.CreateTable(
                name: "TestRuns",
                columns: table => new
                {
                    TestRunId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimeOfRun = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SummaryTestSummaryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRuns", x => x.TestRunId);
                    table.ForeignKey(
                        name: "FK_TestRuns_TestSummary_SummaryTestSummaryId",
                        column: x => x.SummaryTestSummaryId,
                        principalTable: "TestSummary",
                        principalColumn: "TestSummaryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestResult_TestSummaryId",
                table: "TestResult",
                column: "TestSummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_TestRuns_SummaryTestSummaryId",
                table: "TestRuns",
                column: "SummaryTestSummaryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestResult");

            migrationBuilder.DropTable(
                name: "TestRuns");

            migrationBuilder.DropTable(
                name: "TestSummary");
        }
    }
}