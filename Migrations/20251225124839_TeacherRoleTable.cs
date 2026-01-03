using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPSUL.Migrations
{
    /// <inheritdoc />
    public partial class TeacherRoleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Roles_RoleId",
                table: "Teachers");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Teachers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "TeacherRoles",
                columns: table => new
                {
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherRoles", x => new { x.TeacherId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_TeacherRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherRoles_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherRoles_RoleId",
                table: "TeacherRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Roles_RoleId",
                table: "Teachers",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Roles_RoleId",
                table: "Teachers");

            migrationBuilder.DropTable(
                name: "TeacherRoles");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Teachers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Roles_RoleId",
                table: "Teachers",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
