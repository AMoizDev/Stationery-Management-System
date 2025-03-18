using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stationery_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class updatedcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserRoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserRoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.UserRoleId);
                });

            migrationBuilder.CreateTable(
                name: "Stationeries",
                columns: table => new
                {
                    Stationery_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Stationery_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stationery_Quantity = table.Column<int>(type: "int", nullable: false),
                    Stationery_Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Stationery_Price = table.Column<int>(type: "int", nullable: false),
                    Stationery_Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Assign_to = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stationeries", x => x.Stationery_Id);
                    table.ForeignKey(
                        name: "FK_Stationeries_UserRoles_Assign_to",
                        column: x => x.Assign_to,
                        principalTable: "UserRoles",
                        principalColumn: "UserRoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserRole = table.Column<int>(type: "int", nullable: false),
                    UserLimits = table.Column<int>(type: "int", nullable: false),
                    Add_By = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.userId);
                    table.ForeignKey(
                        name: "FK_users_UserRoles_UserRole",
                        column: x => x.UserRole,
                        principalTable: "UserRoles",
                        principalColumn: "UserRoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_users_Add_By",
                        column: x => x.Add_By,
                        principalTable: "users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    requestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    stationaryId = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<int>(type: "int", nullable: false),
                    superior_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.requestId);
                    table.ForeignKey(
                        name: "FK_Requests_Stationeries_stationaryId",
                        column: x => x.stationaryId,
                        principalTable: "Stationeries",
                        principalColumn: "Stationery_Id");
                    table.ForeignKey(
                        name: "FK_Requests_users_superior_id",
                        column: x => x.superior_id,
                        principalTable: "users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_stationaryId",
                table: "Requests",
                column: "stationaryId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_superior_id",
                table: "Requests",
                column: "superior_id");

            migrationBuilder.CreateIndex(
                name: "IX_Stationeries_Assign_to",
                table: "Stationeries",
                column: "Assign_to");

            migrationBuilder.CreateIndex(
                name: "IX_users_Add_By",
                table: "users",
                column: "Add_By");

            migrationBuilder.CreateIndex(
                name: "IX_users_UserRole",
                table: "users",
                column: "UserRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Stationeries");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
