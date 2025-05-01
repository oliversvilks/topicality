using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Topicality.Client.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class tablesupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetUserEmail",
                table: "KnowledgeFLowSteps",
                newName: "TargetCollection");

            migrationBuilder.RenameColumn(
                name: "TargetUserCategory",
                table: "KnowledgeFLowSteps",
                newName: "TargetCategory");

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "UserCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ComparisonSetId",
                table: "KnowledgeFLowSteps",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExecutionTime",
                table: "KnowledgeFLows",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<long>(
                name: "ComparisonSetId",
                table: "Documents",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "CategoryDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Comparisons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComparisonDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ComparisonMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComparisonAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comparisons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComparisonSets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComparisonId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparisonSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComparisonSets_Comparisons_ComparisonId",
                        column: x => x.ComparisonId,
                        principalTable: "Comparisons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeFLowSteps_ComparisonSetId",
                table: "KnowledgeFLowSteps",
                column: "ComparisonSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ComparisonSetId",
                table: "Documents",
                column: "ComparisonSetId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonSets_ComparisonId",
                table: "ComparisonSets",
                column: "ComparisonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_ComparisonSets_ComparisonSetId",
                table: "Documents",
                column: "ComparisonSetId",
                principalTable: "ComparisonSets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeFLowSteps_ComparisonSets_ComparisonSetId",
                table: "KnowledgeFLowSteps",
                column: "ComparisonSetId",
                principalTable: "ComparisonSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_ComparisonSets_ComparisonSetId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeFLowSteps_ComparisonSets_ComparisonSetId",
                table: "KnowledgeFLowSteps");

            migrationBuilder.DropTable(
                name: "ComparisonSets");

            migrationBuilder.DropTable(
                name: "Comparisons");

            migrationBuilder.DropIndex(
                name: "IX_KnowledgeFLowSteps_ComparisonSetId",
                table: "KnowledgeFLowSteps");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ComparisonSetId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "UserCategories");

            migrationBuilder.DropColumn(
                name: "ComparisonSetId",
                table: "KnowledgeFLowSteps");

            migrationBuilder.DropColumn(
                name: "ComparisonSetId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "CategoryDocuments");

            migrationBuilder.RenameColumn(
                name: "TargetCollection",
                table: "KnowledgeFLowSteps",
                newName: "TargetUserEmail");

            migrationBuilder.RenameColumn(
                name: "TargetCategory",
                table: "KnowledgeFLowSteps",
                newName: "TargetUserCategory");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExecutionTime",
                table: "KnowledgeFLows",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
