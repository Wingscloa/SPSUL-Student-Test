using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPSUL.Migrations
{
    /// <inheritdoc />
    public partial class FieldsRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassesFields_StudentFields_StudentFieldId",
                table: "ClassesFields");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_StudentFields_StudentFieldId",
                table: "Tests");

            migrationBuilder.DropTable(
                name: "StudentFields");

            migrationBuilder.CreateTable(
                name: "Fields",
                columns: table => new
                {
                    StudentFieldId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fields", x => x.StudentFieldId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ClassesFields_Fields_StudentFieldId",
                table: "ClassesFields",
                column: "StudentFieldId",
                principalTable: "Fields",
                principalColumn: "StudentFieldId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Fields_StudentFieldId",
                table: "Tests",
                column: "StudentFieldId",
                principalTable: "Fields",
                principalColumn: "StudentFieldId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassesFields_Fields_StudentFieldId",
                table: "ClassesFields");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Fields_StudentFieldId",
                table: "Tests");

            migrationBuilder.DropTable(
                name: "Fields");

            migrationBuilder.CreateTable(
                name: "StudentFields",
                columns: table => new
                {
                    StudentFieldId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFields", x => x.StudentFieldId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ClassesFields_StudentFields_StudentFieldId",
                table: "ClassesFields",
                column: "StudentFieldId",
                principalTable: "StudentFields",
                principalColumn: "StudentFieldId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_StudentFields_StudentFieldId",
                table: "Tests",
                column: "StudentFieldId",
                principalTable: "StudentFields",
                principalColumn: "StudentFieldId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
