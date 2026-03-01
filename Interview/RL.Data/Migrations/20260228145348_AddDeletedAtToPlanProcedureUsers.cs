using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RL.Data.Migrations
{
    public partial class AddDeletedAtToPlanProcedureUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PlanProcedureUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PlanProcedureUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PlanProcedureUsers_IsDeleted",
                table: "PlanProcedureUsers",
                column: "IsDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlanProcedureUsers_IsDeleted",
                table: "PlanProcedureUsers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PlanProcedureUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PlanProcedureUsers");
        }
    }
}
