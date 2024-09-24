using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace workOut.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDoneToDaysTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Day_Workouts_WorkoutId",
                table: "Day");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Day",
                table: "Day");

            migrationBuilder.RenameTable(
                name: "Day",
                newName: "Days");

            migrationBuilder.RenameIndex(
                name: "IX_Day_WorkoutId",
                table: "Days",
                newName: "IX_Days_WorkoutId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "Days",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Days",
                table: "Days",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Days_Workouts_WorkoutId",
                table: "Days",
                column: "WorkoutId",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Days_Workouts_WorkoutId",
                table: "Days");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Days",
                table: "Days");

            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "Days");

            migrationBuilder.RenameTable(
                name: "Days",
                newName: "Day");

            migrationBuilder.RenameIndex(
                name: "IX_Days_WorkoutId",
                table: "Day",
                newName: "IX_Day_WorkoutId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Day",
                table: "Day",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Day_Workouts_WorkoutId",
                table: "Day",
                column: "WorkoutId",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
