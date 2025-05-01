using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Topicality.Client.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class tables_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonSets_Comparisons_ComparisonId",
                table: "ComparisonSets");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeFLowSteps_KnowledgeFLows_KnowledgeFLowId",
                table: "KnowledgeFLowSteps");

            migrationBuilder.DropIndex(
                name: "IX_SchemaDefinitions_CategoryId",
                table: "SchemaDefinitions");

            migrationBuilder.DropColumn(
                name: "TargetCategory",
                table: "KnowledgeFLowSteps");

            migrationBuilder.DropColumn(
                name: "TargetCollection",
                table: "KnowledgeFLowSteps");

            migrationBuilder.RenameColumn(
                name: "KnowledgeFLowId",
                table: "KnowledgeFLowSteps",
                newName: "KnowledgeFlowId");

            migrationBuilder.RenameColumn(
                name: "FLowQuestion",
                table: "KnowledgeFLowSteps",
                newName: "FlowQuestion");

            migrationBuilder.RenameColumn(
                name: "FLowPrompt",
                table: "KnowledgeFLowSteps",
                newName: "FlowPrompt");

            migrationBuilder.RenameColumn(
                name: "IsStrucured",
                table: "KnowledgeFLowSteps",
                newName: "IsStructured");

            migrationBuilder.RenameIndex(
                name: "IX_KnowledgeFLowSteps_KnowledgeFLowId",
                table: "KnowledgeFLowSteps",
                newName: "IX_KnowledgeFLowSteps_KnowledgeFlowId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Answer",
                table: "DocumentShares",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prompt",
                table: "DocumentShares",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ShowAsWeb",
                table: "DocumentShares",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "DocumentShares",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "WebTemplate",
                table: "DocumentShares",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<long>(
                name: "ComparisonId",
                table: "ComparisonSets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchemaDefinitions_CategoryId",
                table: "SchemaDefinitions",
                column: "CategoryId",
                unique: true,
                filter: "[CategoryId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonSets_Comparisons_ComparisonId",
                table: "ComparisonSets",
                column: "ComparisonId",
                principalTable: "Comparisons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeFLowSteps_KnowledgeFLows_KnowledgeFlowId",
                table: "KnowledgeFLowSteps",
                column: "KnowledgeFlowId",
                principalTable: "KnowledgeFLows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComparisonSets_Comparisons_ComparisonId",
                table: "ComparisonSets");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeFLowSteps_KnowledgeFLows_KnowledgeFlowId",
                table: "KnowledgeFLowSteps");

            migrationBuilder.DropIndex(
                name: "IX_SchemaDefinitions_CategoryId",
                table: "SchemaDefinitions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserCategories");

            migrationBuilder.DropColumn(
                name: "Answer",
                table: "DocumentShares");

            migrationBuilder.DropColumn(
                name: "Prompt",
                table: "DocumentShares");

            migrationBuilder.DropColumn(
                name: "ShowAsWeb",
                table: "DocumentShares");

            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "DocumentShares");

            migrationBuilder.DropColumn(
                name: "WebTemplate",
                table: "DocumentShares");

            migrationBuilder.RenameColumn(
                name: "KnowledgeFlowId",
                table: "KnowledgeFLowSteps",
                newName: "KnowledgeFLowId");

            migrationBuilder.RenameColumn(
                name: "FlowQuestion",
                table: "KnowledgeFLowSteps",
                newName: "FLowQuestion");

            migrationBuilder.RenameColumn(
                name: "FlowPrompt",
                table: "KnowledgeFLowSteps",
                newName: "FLowPrompt");

            migrationBuilder.RenameColumn(
                name: "IsStructured",
                table: "KnowledgeFLowSteps",
                newName: "IsStrucured");

            migrationBuilder.RenameIndex(
                name: "IX_KnowledgeFLowSteps_KnowledgeFlowId",
                table: "KnowledgeFLowSteps",
                newName: "IX_KnowledgeFLowSteps_KnowledgeFLowId");

            migrationBuilder.AddColumn<string>(
                name: "TargetCategory",
                table: "KnowledgeFLowSteps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TargetCollection",
                table: "KnowledgeFLowSteps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<long>(
                name: "ComparisonId",
                table: "ComparisonSets",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaDefinitions_CategoryId",
                table: "SchemaDefinitions",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComparisonSets_Comparisons_ComparisonId",
                table: "ComparisonSets",
                column: "ComparisonId",
                principalTable: "Comparisons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeFLowSteps_KnowledgeFLows_KnowledgeFLowId",
                table: "KnowledgeFLowSteps",
                column: "KnowledgeFLowId",
                principalTable: "KnowledgeFLows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
