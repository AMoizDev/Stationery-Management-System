using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stationery_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class dbupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserRoleId", "UserRoleName" },
                values: new object[] { 1, "Admin" });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "userId", "Add_By", "UserEmail", "UserLimits", "UserName", "UserPassword", "UserPhone", "UserRole" },
                values: new object[] { 1, 1, "Admin@gmail.com", 100000, "Admin", "123456789", "03123456789", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "userId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "UserRoleId",
                keyValue: 1);
        }
    }
}
