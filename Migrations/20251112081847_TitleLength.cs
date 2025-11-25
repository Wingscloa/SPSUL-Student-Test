using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPSUL.Migrations
{
    /// <inheritdoc />
    public partial class TitleLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Titles",
                type: "nvarchar(48)",
                maxLength: 48,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Titles",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(48)",
                oldMaxLength: 48);
        }
    }
}
