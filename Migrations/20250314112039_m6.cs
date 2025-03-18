using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stationery_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class m6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stationeries_UserRoles_Assign_to",
                table: "Stationeries");

            migrationBuilder.AlterColumn<int>(
                name: "Assign_to",
                table: "Stationeries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Stationeries_UserRoles_Assign_to",
                table: "Stationeries",
                column: "Assign_to",
                principalTable: "UserRoles",
                principalColumn: "UserRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stationeries_UserRoles_Assign_to",
                table: "Stationeries");

            migrationBuilder.AlterColumn<int>(
                name: "Assign_to",
                table: "Stationeries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stationeries_UserRoles_Assign_to",
                table: "Stationeries",
                column: "Assign_to",
                principalTable: "UserRoles",
                principalColumn: "UserRoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
