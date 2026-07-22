using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoApp.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Goals_ParentGoalId",
                table: "Goals");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Goals_GoalId",
                table: "Tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Goals_ParentGoalId",
                table: "Goals",
                column: "ParentGoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Goals_GoalId",
                table: "Tasks",
                column: "GoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Goals_ParentGoalId",
                table: "Goals");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Goals_GoalId",
                table: "Tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Goals_ParentGoalId",
                table: "Goals",
                column: "ParentGoalId",
                principalTable: "Goals",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Goals_GoalId",
                table: "Tasks",
                column: "GoalId",
                principalTable: "Goals",
                principalColumn: "Id");
        }
    }
}
