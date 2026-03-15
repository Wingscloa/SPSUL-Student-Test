using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SPSUL.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    TeacherName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Detail = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    ClassesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    StartFrom = table.Column<int>(type: "int", nullable: false),
                    EndTo = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.ClassesId);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.PermissionId);
                });

            migrationBuilder.CreateTable(
                name: "QuestionTypes",
                columns: table => new
                {
                    QuestionTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTypes", x => x.QuestionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "StudentFields",
                columns: table => new
                {
                    StudentFieldId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFields", x => x.StudentFieldId);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    TeacherId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    NickName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.TeacherId);
                });

            migrationBuilder.CreateTable(
                name: "Titles",
                columns: table => new
                {
                    TitleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Shortcut = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Titles", x => x.TitleId);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.PermissionId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassesFields",
                columns: table => new
                {
                    ClassesId = table.Column<int>(type: "int", nullable: false),
                    StudentFieldId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassesFields", x => new { x.ClassesId, x.StudentFieldId });
                    table.ForeignKey(
                        name: "FK_ClassesFields_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalTable: "Classes",
                        principalColumn: "ClassesId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassesFields_StudentFields_StudentFieldId",
                        column: x => x.StudentFieldId,
                        principalTable: "StudentFields",
                        principalColumn: "StudentFieldId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassesStudents",
                columns: table => new
                {
                    ClassesId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassesStudents", x => new { x.ClassesId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_ClassesStudents_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalTable: "Classes",
                        principalColumn: "ClassesId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassesStudents_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Header = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    QuestionTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Questions_QuestionTypes_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalTable: "QuestionTypes",
                        principalColumn: "QuestionTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Questions_StudentFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "StudentFields",
                        principalColumn: "StudentFieldId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Questions_Teachers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Teachers",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    TestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    StudentFieldId = table.Column<int>(type: "int", nullable: false),
                    QuestionSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeLimitMinutes = table.Column<int>(type: "int", nullable: true),
                    ShuffleQuestions = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.TestId);
                    table.ForeignKey(
                        name: "FK_Tests_StudentFields_StudentFieldId",
                        column: x => x.StudentFieldId,
                        principalTable: "StudentFields",
                        principalColumn: "StudentFieldId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tests_Teachers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Teachers",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherTitles",
                columns: table => new
                {
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    TitleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherTitles", x => new { x.TeacherId, x.TitleId });
                    table.ForeignKey(
                        name: "FK_TeacherTitles_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "TeacherId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherTitles_Titles_TitleId",
                        column: x => x.TitleId,
                        principalTable: "Titles",
                        principalColumn: "TitleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    QuestionOptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    ImageKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Text = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.QuestionOptionId);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentTests",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    LoginId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResultSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShuffleOrder = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTests", x => new { x.StudentId, x.TestId });
                    table.ForeignKey(
                        name: "FK_StudentTests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentTests_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "TestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "ClassesId", "EndTo", "IsActive", "Name", "StartFrom" },
                values: new object[,]
                {
                    { 1, 2029, true, "1.A", 2025 },
                    { 2, 2029, true, "1.B", 2025 },
                    { 3, 2028, true, "2.A", 2024 },
                    { 4, 2028, true, "2.B", 2024 },
                    { 5, 2027, true, "3.A", 2023 }
                });

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "ClassesId", "EndTo", "Name", "StartFrom" },
                values: new object[] { 6, 2026, "4.A", 2022 });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "All Permissions" },
                    { 2, true, "CURD" },
                    { 3, true, "CRUD Test" },
                    { 4, true, "CRUD Teacher" },
                    { 5, true, "CRUD Student" },
                    { 6, true, "View" }
                });

            migrationBuilder.InsertData(
                table: "QuestionTypes",
                columns: new[] { "QuestionTypeId", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "Vyber z moznosti" },
                    { 2, true, "Uzavrena otazka s obrazky" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Opravneni udeluje plnou kontrolu nad systemem – spravce muze vytvaret, upravovat i mazat vsechny ucty.", true, "Administrator" },
                    { 2, "Opravneni udeluje moznost vytvareni, aktualizovani a cteni vsech systemu v aplikaci, krom ucitelu.", true, "Tvurce" },
                    { 3, "Opravneni udeluje moznost vsechny operace pro system testu.", true, "Testator" },
                    { 4, "Opravneni udeluje moznost vsechny operace pro system ucitelu.", true, "Ucitelator" },
                    { 5, "Opravneni udeluje moznost vsechny operace pro system studentu.", true, "Studentator" },
                    { 6, "Opravneni udeluje pohled na vsechny systemy.", true, "Hledic" }
                });

            migrationBuilder.InsertData(
                table: "StudentFields",
                columns: new[] { "StudentFieldId", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "Anglicky jazyk" },
                    { 2, true, "Databaze" },
                    { 3, true, "Ekonomika" },
                    { 4, true, "Elektrotechnika" },
                    { 5, true, "Fyzika" },
                    { 6, true, "Kyberbezpecnost" },
                    { 7, true, "Matematicky seminar" },
                    { 8, true, "Matematika" },
                    { 9, true, "Operacni systemy" },
                    { 10, true, "Praxe" },
                    { 11, true, "Programovani a vyvoj aplikaci" },
                    { 12, true, "Projekty" },
                    { 13, true, "Telesna vychova" },
                    { 14, true, "Cesky jazyk a literatura" },
                    { 15, true, "Zaklady elektrotechniky" },
                    { 16, true, "Materialy a technologie" },
                    { 17, true, "Informacni a komunikacni technologie" },
                    { 18, true, "Nauka o spolecnosti" },
                    { 19, true, "Odborny vycvik" },
                    { 20, true, "Elektricke stroje a pristroje" },
                    { 21, true, "Technicka dokumentace" },
                    { 22, true, "Dejepis" },
                    { 23, true, "Aplikacni software" },
                    { 24, true, "Webove aplikace" },
                    { 25, true, "Zaklady prirodnich ved" },
                    { 26, true, "Algoritmizace" },
                    { 27, true, "Datove site" },
                    { 28, true, "Logistika" },
                    { 29, true, "Doprava" },
                    { 30, true, "Nemecky jazyk" },
                    { 31, true, "Pisemna a elektronicka komunikace" },
                    { 32, true, "Obcanska nauka" },
                    { 33, true, "Automatizace" },
                    { 34, true, "Elektronika" },
                    { 35, true, "Elektrotechnicka mereni" },
                    { 36, true, "Herni vyvoj" },
                    { 37, true, "Marketing a management" },
                    { 38, true, "Ucetnictvi" },
                    { 39, true, "Webove technologie" },
                    { 40, true, "Zemepis" },
                    { 41, true, "Strojnictvi" },
                    { 42, true, "Programovani" },
                    { 43, true, "Elektronika a sdelovaci technika" },
                    { 44, true, "Cislicova technika" },
                    { 45, true, "Automatizace" },
                    { 46, true, "Mikroprocesorova technika" },
                    { 47, true, "Technicke kresleni" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "FirstName", "IsActive", "LastName" },
                values: new object[,]
                {
                    { 1, "Jakub", true, "Horak" },
                    { 2, "Tereza", true, "Markova" },
                    { 3, "Martin", true, "Jelinek" },
                    { 4, "Lucie", true, "Pokorna" },
                    { 5, "David", true, "Ruzicka" },
                    { 6, "Anna", true, "Benesova" },
                    { 7, "Ondrej", true, "Fiala" },
                    { 8, "Karolina", true, "Stastna" },
                    { 9, "Vojtech", true, "Kucera" },
                    { 10, "Eliska", true, "Vesela" },
                    { 11, "Matej", true, "Marek" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "FirstName", "LastName" },
                values: new object[] { 12, "Natalie", "Kopecka" });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "FirstName", "IsActive", "LastName" },
                values: new object[,]
                {
                    { 13, "Stepan", true, "Havlicek" },
                    { 14, "Michaela", true, "Vlckova" },
                    { 15, "Adam", true, "Bartos" },
                    { 16, "Barbora", true, "Urbanova" },
                    { 17, "Daniel", true, "Blazek" },
                    { 18, "Kristyna", true, "Sedlackova" },
                    { 19, "Filip", true, "Kratochvil" },
                    { 20, "Simona", true, "Nemcova" },
                    { 21, "Lukas", true, "Pospisil" },
                    { 22, "Veronika", true, "Holubova" },
                    { 23, "Dominik", true, "Simek" },
                    { 24, "Klara", true, "Dostalova" }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "TeacherId", "FirstName", "LastName", "NickName", "PasswordHash" },
                values: new object[,]
                {
                    { 1, "Admin", "Admin", "Admin", "$2b$10$jErDDvlTESkhHfdiHuRFte9ojuRZNZST.gskJ4PVgp6h6q0VGmVxS" },
                    { 2, "Filip", "Eder", "FilipEder", "$2b$10$PMAUpgwQ3pwpaIMv1CCYA.YzbHYl5.PXqmsRNBaR1Oevp3m7w1v2y" },
                    { 3, "Petr", "Novak", "PetrNovak", "$2b$10$YqPz7WEHhmjRpRuFqaVPVu505tO1z4KwGVnnj3T3J0S9SEnZMrZSG" },
                    { 4, "Jana", "Svobodova", "JanaSvobodova", "$2b$10$PMAUpgwQ3pwpaIMv1CCYA.YzbHYl5.PXqmsRNBaR1Oevp3m7w1v2y" },
                    { 5, "Tomas", "Dvorak", "TomasDvorak", "$2b$10$PMAUpgwQ3pwpaIMv1CCYA.YzbHYl5.PXqmsRNBaR1Oevp3m7w1v2y" },
                    { 6, "Marie", "Cerna", "MarieCerna", "$2b$10$PMAUpgwQ3pwpaIMv1CCYA.YzbHYl5.PXqmsRNBaR1Oevp3m7w1v2y" },
                    { 7, "Jan", "Prochazka", "JanProchazka", "$2b$10$PMAUpgwQ3pwpaIMv1CCYA.YzbHYl5.PXqmsRNBaR1Oevp3m7w1v2y" },
                    { 8, "Eva", "Krejci", "EvaKrejci", "$2b$10$PMAUpgwQ3pwpaIMv1CCYA.YzbHYl5.PXqmsRNBaR1Oevp3m7w1v2y" }
                });

            migrationBuilder.InsertData(
                table: "Titles",
                columns: new[] { "TitleId", "IsActive", "Name", "Shortcut" },
                values: new object[,]
                {
                    { 1, true, "Bakalar", "Bc." },
                    { 2, true, "Magistr", "Mgr." },
                    { 3, true, "Inzenyr", "Ing." },
                    { 4, true, "Doktor filozofie", "PhDr." },
                    { 5, true, "Doktor prav", "JUDr." },
                    { 6, true, "Doktor prirodnich ved", "RNDr." },
                    { 7, true, "Doktor filozofie", "Ph.D." },
                    { 8, true, "Doktor teologie", "Th.D." },
                    { 9, true, "Magistersky titul obchodni administrativy", "MBA" },
                    { 10, true, "Magistr prav", "LL.M." }
                });

            migrationBuilder.InsertData(
                table: "ClassesFields",
                columns: new[] { "ClassesId", "StudentFieldId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 8 },
                    { 1, 11 },
                    { 1, 17 },
                    { 1, 26 },
                    { 2, 1 },
                    { 2, 4 },
                    { 2, 5 },
                    { 2, 8 },
                    { 2, 15 },
                    { 3, 2 },
                    { 3, 8 },
                    { 3, 9 },
                    { 3, 11 },
                    { 3, 24 },
                    { 4, 4 },
                    { 4, 8 },
                    { 4, 33 },
                    { 4, 34 },
                    { 5, 6 },
                    { 5, 12 },
                    { 5, 27 },
                    { 5, 42 },
                    { 6, 10 },
                    { 6, 12 },
                    { 6, 39 },
                    { 6, 42 }
                });

            migrationBuilder.InsertData(
                table: "ClassesStudents",
                columns: new[] { "ClassesId", "StudentId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 2, 5 },
                    { 2, 6 },
                    { 2, 7 },
                    { 2, 8 },
                    { 3, 9 },
                    { 3, 10 },
                    { 3, 11 },
                    { 3, 12 },
                    { 4, 13 },
                    { 4, 14 },
                    { 4, 15 },
                    { 4, 16 },
                    { 5, 17 },
                    { 5, 18 },
                    { 5, 19 },
                    { 5, 20 },
                    { 6, 21 },
                    { 6, 22 },
                    { 6, 23 },
                    { 6, 24 }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "QuestionId", "CreatorId", "Description", "FieldId", "Header", "IsActive", "QuestionTypeId" },
                values: new object[,]
                {
                    { 1, 4, "Zakladni koncept programovani — vysvetlete, co znamena pojem promenna.", 11, "Co je to promenna?", true, 1 },
                    { 2, 4, "Datove typy v jazyce C# — vyberte spravny celociselny typ.", 11, "Ktery datovy typ je celociselny v C#?", true, 1 },
                    { 3, 4, "Ridici struktury v programovani — podminene vetveni.", 11, "Co dela prikaz 'if'?", true, 1 },
                    { 4, 7, "Cykly v programovani — opakovani bloku kodu.", 11, "Co je to cyklus 'for'?", true, 1 },
                    { 5, 7, "Zaklady databazovych jazyku — puvod zkratky SQL.", 2, "Co znamena zkratka SQL?", true, 1 },
                    { 6, 7, "SQL prikazy pro cteni dat z databaze.", 2, "Ktery prikaz slouzi k vyberu dat?", true, 1 },
                    { 7, 7, "Integritni omezeni v relacnich databazich.", 2, "Co je primarni klic?", true, 1 },
                    { 8, 4, "Spojovani tabulek v SQL dotazech.", 2, "Co dela prikaz JOIN?", true, 1 },
                    { 9, 4, "Socialni inzenyrstvi — rozpoznavani phishingovych utoku.", 6, "Co je phishing?", true, 1 },
                    { 10, 4, "Sitova bezpecnost — ochrana perimetru site.", 6, "Co je to firewall?", true, 1 },
                    { 11, 1, "Mocniny — vypocet treti mocniny cisla 2.", 8, "Kolik je 2^3?", true, 1 }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "QuestionId", "CreatorId", "Description", "FieldId", "Header", "QuestionTypeId" },
                values: new object[] { 12, 4, "Tato otazka byla deaktivovana a nemela by se zobrazovat v testech.", 11, "Zastarala otazka", 1 });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 6, 6 }
                });

            migrationBuilder.InsertData(
                table: "TeacherRoles",
                columns: new[] { "RoleId", "TeacherId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 6, 2 },
                    { 2, 4 },
                    { 3, 5 },
                    { 5, 6 },
                    { 2, 7 },
                    { 4, 8 }
                });

            migrationBuilder.InsertData(
                table: "TeacherTitles",
                columns: new[] { "TeacherId", "TitleId" },
                values: new object[,]
                {
                    { 4, 2 },
                    { 5, 3 },
                    { 6, 1 },
                    { 7, 3 },
                    { 7, 7 },
                    { 8, 2 }
                });

            migrationBuilder.InsertData(
                table: "Tests",
                columns: new[] { "TestId", "CreatorId", "IsActive", "Name", "QuestionSnapshot", "ShuffleQuestions", "StudentFieldId", "TimeLimitMinutes" },
                values: new object[,]
                {
                    { 1, 4, true, "PVA — Zaklady programovani", "[{\"QuestionId\":1,\"Header\":\"Co je to promenna?\",\"Description\":\"Zakladni koncept programovani.\",\"QuestionType\":\"Vyber z moznosti\",\"Options\":[{\"Text\":\"Funkce pro vypocet\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Pojmenovane misto v pameti pro ulozeni hodnoty\",\"ImageKey\":null,\"IsCorrect\":true},{\"Text\":\"Typ souboru\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Prikaz pro vystup na obrazovku\",\"ImageKey\":null,\"IsCorrect\":false}]},{\"QuestionId\":2,\"Header\":\"Ktery datovy typ je celociselny v C#?\",\"Description\":\"Datove typy v jazyce C#.\",\"QuestionType\":\"Vyber z moznosti\",\"Options\":[{\"Text\":\"number\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"text\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"int\",\"ImageKey\":null,\"IsCorrect\":true},{\"Text\":\"letra\",\"ImageKey\":null,\"IsCorrect\":false}]},{\"QuestionId\":3,\"Header\":\"Co dela prikaz 'if'?\",\"Description\":\"Ridici struktury.\",\"QuestionType\":\"Vyber z moznosti\",\"Options\":[{\"Text\":\"Opakuje blok kodu\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Definuje novou funkci\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Vykona kod pouze pokud je podminka pravdiva\",\"ImageKey\":null,\"IsCorrect\":true},{\"Text\":\"Ukonci program\",\"ImageKey\":null,\"IsCorrect\":false}]},{\"QuestionId\":4,\"Header\":\"Co je to cyklus 'for'?\",\"Description\":\"Cykly v programovani.\",\"QuestionType\":\"Vyber z moznosti\",\"Options\":[{\"Text\":\"Podmineny prikaz\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Cyklus s predem danym poctem opakovani\",\"ImageKey\":null,\"IsCorrect\":true},{\"Text\":\"Deklarace promenne\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Import knihovny\",\"ImageKey\":null,\"IsCorrect\":false}]}]", true, 11, 15 },
                    { 2, 7, true, "Databaze — SQL zaklady", "[{\"QuestionId\":5,\"Header\":\"Co znamena zkratka SQL?\",\"Description\":\"Zaklady databazovych jazyku.\",\"QuestionType\":\"Vyber z moznosti\",\"Options\":[{\"Text\":\"Standard Query Language\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Structured Query Language\",\"ImageKey\":null,\"IsCorrect\":true},{\"Text\":\"System Query Logic\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Simple Question Language\",\"ImageKey\":null,\"IsCorrect\":false}]},{\"QuestionId\":6,\"Header\":\"Ktery prikaz slouzi k vyberu dat?\",\"Description\":\"SQL prikazy.\",\"QuestionType\":\"Vyber z moznosti\",\"Options\":[{\"Text\":\"INSERT\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"UPDATE\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"SELECT\",\"ImageKey\":null,\"IsCorrect\":true},{\"Text\":\"DELETE\",\"ImageKey\":null,\"IsCorrect\":false}]},{\"QuestionId\":7,\"Header\":\"Co je primarni klic?\",\"Description\":\"Integritni omezeni.\",\"QuestionType\":\"Vyber z moznosti\",\"Options\":[{\"Text\":\"Heslo do databaze\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Unikatni identifikator zaznamu v tabulce\",\"ImageKey\":null,\"IsCorrect\":true},{\"Text\":\"Nazev tabulky\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Typ sloupce\",\"ImageKey\":null,\"IsCorrect\":false}]},{\"QuestionId\":8,\"Header\":\"Co dela prikaz JOIN?\",\"Description\":\"Spojovani tabulek.\",\"QuestionType\":\"Vyber z moznosti\",\"Options\":[{\"Text\":\"Maze data ze dvou tabulek\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Spojuje data z vice tabulek na zaklade podminky\",\"ImageKey\":null,\"IsCorrect\":true},{\"Text\":\"Vytvari novou tabulku\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Radi vysledky vzestupne\",\"ImageKey\":null,\"IsCorrect\":false}]}]", false, 2, 20 }
                });

            migrationBuilder.InsertData(
                table: "Tests",
                columns: new[] { "TestId", "CreatorId", "Name", "QuestionSnapshot", "ShuffleQuestions", "StudentFieldId", "TimeLimitMinutes" },
                values: new object[] { 3, 4, "Kyberbezpecnost — Uvod", "[{\"QuestionId\":9,\"Header\":\"Co je phishing?\",\"Description\":\"Socialni inzenyrstvi.\",\"QuestionType\":\"Vyber z moznosti\",\"Options\":[{\"Text\":\"Antivirovy program\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Podvodny pokus o ziskani citlivych udaju\",\"ImageKey\":null,\"IsCorrect\":true},{\"Text\":\"Sifrovaci algoritmus\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Typ sitoveho kabelu\",\"ImageKey\":null,\"IsCorrect\":false}]},{\"QuestionId\":10,\"Header\":\"Co je to firewall?\",\"Description\":\"Sitova bezpecnost.\",\"QuestionType\":\"Vyber z moznosti\",\"Options\":[{\"Text\":\"Hardware pro tisk\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Zarizeni/software filtrujici sitovy provoz\",\"ImageKey\":null,\"IsCorrect\":true},{\"Text\":\"Typ operacniho systemu\",\"ImageKey\":null,\"IsCorrect\":false},{\"Text\":\"Programovaci jazyk\",\"ImageKey\":null,\"IsCorrect\":false}]}]", true, 6, null });

            migrationBuilder.InsertData(
                table: "QuestionOptions",
                columns: new[] { "QuestionOptionId", "ImageKey", "IsCorrect", "QuestionId", "Text" },
                values: new object[,]
                {
                    { 1, "", false, 1, "Funkce pro vypocet" },
                    { 2, "", true, 1, "Pojmenovane misto v pameti pro ulozeni hodnoty" },
                    { 3, "", false, 1, "Typ souboru" },
                    { 4, "", false, 1, "Prikaz pro vystup na obrazovku" },
                    { 5, "", false, 2, "number" },
                    { 6, "", false, 2, "text" },
                    { 7, "", true, 2, "int" },
                    { 8, "", false, 2, "letra" },
                    { 9, "", false, 3, "Opakuje blok kodu" },
                    { 10, "", false, 3, "Definuje novou funkci" },
                    { 11, "", true, 3, "Vykona kod pouze pokud je podminka pravdiva" },
                    { 12, "", false, 3, "Ukonci program" },
                    { 13, "", false, 4, "Podmineny prikaz" },
                    { 14, "", true, 4, "Cyklus s predem danym poctem opakovani" },
                    { 15, "", false, 4, "Deklarace promenne" },
                    { 16, "", false, 4, "Import knihovny" },
                    { 17, "", false, 5, "Standard Query Language" },
                    { 18, "", true, 5, "Structured Query Language" },
                    { 19, "", false, 5, "System Query Logic" },
                    { 20, "", false, 5, "Simple Question Language" },
                    { 21, "", false, 6, "INSERT" },
                    { 22, "", false, 6, "UPDATE" },
                    { 23, "", true, 6, "SELECT" },
                    { 24, "", false, 6, "DELETE" },
                    { 25, "", false, 7, "Heslo do databaze" },
                    { 26, "", true, 7, "Unikatni identifikator zaznamu v tabulce" },
                    { 27, "", false, 7, "Nazev tabulky" },
                    { 28, "", false, 7, "Typ sloupce" },
                    { 29, "", false, 8, "Maze data ze dvou tabulek" },
                    { 30, "", true, 8, "Spojuje data z vice tabulek na zaklade podminky" },
                    { 31, "", false, 8, "Vytvari novou tabulku" },
                    { 32, "", false, 8, "Radi vysledky vzestupne" },
                    { 33, "", false, 9, "Antivirovy program" },
                    { 34, "", true, 9, "Podvodny pokus o ziskani citlivych udaju" },
                    { 35, "", false, 9, "Sifrovaci algoritmus" },
                    { 36, "", false, 9, "Typ sitoveho kabelu" },
                    { 37, "", false, 10, "Hardware pro tisk" },
                    { 38, "", true, 10, "Zarizeni/software filtrujici sitovy provoz" },
                    { 39, "", false, 10, "Typ operacniho systemu" },
                    { 40, "", false, 10, "Programovaci jazyk" },
                    { 41, "", false, 11, "6" },
                    { 42, "", true, 11, "8" },
                    { 43, "", false, 11, "9" },
                    { 44, "", false, 11, "4" },
                    { 45, "", false, 12, "Odpoved A" },
                    { 46, "", true, 12, "Odpoved B" },
                    { 47, "", false, 12, "Odpoved C" },
                    { 48, "", false, 12, "Odpoved D" }
                });

            migrationBuilder.InsertData(
                table: "StudentTests",
                columns: new[] { "StudentId", "TestId", "FinishedAt", "LoginId", "ResultSnapshot", "ShuffleOrder", "StartedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 3, 10, 8, 12, 0, 0, DateTimeKind.Utc), "PVA-HOR-001", "{\"Answers\":[{\"QuestionId\":1,\"SelectedOptions\":[\"Pojmenovane misto v pameti pro ulozeni hodnoty\"]},{\"QuestionId\":2,\"SelectedOptions\":[\"int\"]},{\"QuestionId\":3,\"SelectedOptions\":[\"Vykona kod pouze pokud je podminka pravdiva\"]},{\"QuestionId\":4,\"SelectedOptions\":[\"Cyklus s predem danym poctem opakovani\"]}],\"CurrentQuestionIndex\":3}", null, new DateTime(2026, 3, 10, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PVA-MAR-002", "{\"Answers\":[{\"QuestionId\":1,\"SelectedOptions\":[\"Pojmenovane misto v pameti pro ulozeni hodnoty\"]},{\"QuestionId\":2,\"SelectedOptions\":[\"text\"]}],\"CurrentQuestionIndex\":2}", null, new DateTime(2026, 3, 10, 8, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "PVA-JEL-003", "{\"Answers\":[],\"CurrentQuestionIndex\":0}", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, 2, new DateTime(2026, 3, 12, 10, 18, 0, 0, DateTimeKind.Utc), "SQL-KUC-009", "{\"Answers\":[{\"QuestionId\":5,\"SelectedOptions\":[\"Structured Query Language\"]},{\"QuestionId\":6,\"SelectedOptions\":[\"SELECT\"]},{\"QuestionId\":7,\"SelectedOptions\":[\"Nazev tabulky\"]},{\"QuestionId\":8,\"SelectedOptions\":[\"Spojuje data z vice tabulek na zaklade podminky\"]}],\"CurrentQuestionIndex\":3}", null, new DateTime(2026, 3, 12, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 2, new DateTime(2026, 3, 12, 10, 15, 0, 0, DateTimeKind.Utc), "SQL-VES-010", "{\"Answers\":[{\"QuestionId\":5,\"SelectedOptions\":[\"Structured Query Language\"]},{\"QuestionId\":6,\"SelectedOptions\":[\"SELECT\"]},{\"QuestionId\":7,\"SelectedOptions\":[\"Unikatni identifikator zaznamu v tabulce\"]},{\"QuestionId\":8,\"SelectedOptions\":[\"Spojuje data z vice tabulek na zaklade podminky\"]}],\"CurrentQuestionIndex\":3}", null, new DateTime(2026, 3, 12, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "SQL-MAR-011", "{\"Answers\":[],\"CurrentQuestionIndex\":0}", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17, 3, new DateTime(2026, 2, 20, 9, 8, 0, 0, DateTimeKind.Utc), "KYB-BLA-017", "{\"Answers\":[{\"QuestionId\":9,\"SelectedOptions\":[\"Podvodny pokus o ziskani citlivych udaju\"]},{\"QuestionId\":10,\"SelectedOptions\":[\"Zarizeni/software filtrujici sitovy provoz\"]}],\"CurrentQuestionIndex\":1}", null, new DateTime(2026, 2, 20, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, 3, new DateTime(2026, 2, 20, 9, 5, 0, 0, DateTimeKind.Utc), "KYB-SED-018", "{\"Answers\":[{\"QuestionId\":9,\"SelectedOptions\":[\"Sifrovaci algoritmus\"]},{\"QuestionId\":10,\"SelectedOptions\":[\"Zarizeni/software filtrujici sitovy provoz\"]}],\"CurrentQuestionIndex\":1}", null, new DateTime(2026, 2, 20, 9, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ClassesFields_StudentFieldId",
                table: "ClassesFields",
                column: "StudentFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassesStudents_StudentId",
                table: "ClassesStudents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId",
                table: "QuestionOptions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CreatorId",
                table: "Questions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_FieldId",
                table: "Questions",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionTypeId",
                table: "Questions",
                column: "QuestionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTests_TestId",
                table: "StudentTests",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherRoles_RoleId",
                table: "TeacherRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTitles_TitleId",
                table: "TeacherTitles",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_CreatorId",
                table: "Tests",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_StudentFieldId",
                table: "Tests",
                column: "StudentFieldId");


            migrationBuilder.Sql("""
                    CREATE VIEW [dbo].[QuestionRow] AS
                    SELECT  
                        Q.QuestionId, 
                        Q.Header, 
                        Q.Description,
                        CONCAT(
                            ISNULL((SELECT STRING_AGG(DistinctTitles.Shortcut, ' ') WITHIN GROUP (ORDER BY DistinctTitles.Shortcut)
                                    FROM (SELECT DISTINCT TT.TeacherId, Titles.TitleId, Titles.Shortcut
                                        FROM TeacherTitles AS TT
                                        JOIN Titles ON Titles.TitleId = TT.TitleId
                                        WHERE TT.TeacherId = T.TeacherId) AS DistinctTitles), ''),
                            ' ',
                            T.FirstName, 
                            ' ',
                            T.LastName
                        ) AS CreatorName,
                        COUNT(QuestionOptions.QuestionId) AS OptionCount,
                        QuestionTypes.Name AS 'QuestionTypeName',
                        StudentFields.Name AS 'FieldName',
                        Q.IsActive
                FROM Questions AS Q
                JOIN Teachers AS T ON Q.CreatorId = T.TeacherId
                LEFT JOIN QuestionOptions ON Q.QuestionId = QuestionOptions.QuestionId
                LEFT JOIN QuestionTypes ON Q.QuestionTypeId = QuestionTypes.QuestionTypeId
                LEFT JOIN StudentFields ON Q.FieldId = StudentFields.StudentFieldId
                GROUP BY Q.QuestionId, Q.Header, T.FirstName, T.LastName, T.TeacherId, QuestionTypes.Name, StudentFields.Name, Q.IsActive, Q.Description
                """);

            migrationBuilder.Sql("""
                    IF OBJECT_ID(N'[dbo].[Sessions]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[Sessions](
                        [Id] NVARCHAR(449) NOT NULL,
                        [Value] VARBINARY(MAX) NOT NULL,
                        [ExpiresAtTime] DATETIMEOFFSET NOT NULL,
                        [SlidingExpirationInSeconds] BIGINT NULL,
                        [AbsoluteExpiration] DATETIMEOFFSET NULL,
                        CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id])
                    );
                    CREATE INDEX [IX_Sessions_ExpiresAtTime] ON [dbo].[Sessions] ([ExpiresAtTime]);
                END
                """);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS [dbo].[QuestionRow]");

            migrationBuilder.Sql("DROP TABLE IF EXISTS [dbo].[Sessions]");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ClassesFields");

            migrationBuilder.DropTable(
                name: "ClassesStudents");

            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "StudentTests");

            migrationBuilder.DropTable(
                name: "TeacherRoles");

            migrationBuilder.DropTable(
                name: "TeacherTitles");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Titles");

            migrationBuilder.DropTable(
                name: "QuestionTypes");

            migrationBuilder.DropTable(
                name: "StudentFields");

            migrationBuilder.DropTable(
                name: "Teachers");
        }
    }
}
