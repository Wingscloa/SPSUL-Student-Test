using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPSUL.Migrations
{
    /// <inheritdoc />
    public partial class MigrationTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Roles_RoleId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_RoleId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Teachers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Teachers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_RoleId",
                table: "Teachers",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Roles_RoleId",
                table: "Teachers",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId");
        }
    }
}
