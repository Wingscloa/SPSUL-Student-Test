using Microsoft.EntityFrameworkCore;
using SPSUL.Models.Data;
using SPSUL.Models.Display.QuestionModels;

namespace SPSUL.Models
{
    public class SpsulContext : DbContext
    {
        public SpsulContext(DbContextOptions<SpsulContext> options) : base(options)
        {

        }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<TeacherTitle> TeacherTitles { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Classes> Classes { get; set; }
        public DbSet<ClassesStudent> ClassesStudents { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentField> StudentFields { get; set; }
        public DbSet<StudentTest> StudentTests { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TeacherRole> TeacherRoles { get; set; }
        public DbSet<ClassesFields> ClassesFields { get; set; }
        public DbSet<QuestionRow> QuestionRow { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>(e =>
            {
                e.HasKey(e => e.TeacherId);

                e.Property(e => e.LastName).HasMaxLength(64);
                e.Property(e => e.FirstName).HasMaxLength(64);
                e.Property(e => e.NickName).HasMaxLength(64);
                e.Property(e => e.PasswordHash).HasMaxLength(255);
                e.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<TeacherTitle>(e =>
            {
                e.HasKey(e => new { e.TeacherId, e.TitleId });

                e.HasOne(e => e.Teacher).WithMany(e => e.Titles).HasForeignKey(e => e.TeacherId);
                e.HasOne(e => e.Title).WithMany(e => e.TeacherTitles).HasForeignKey(e => e.TitleId);
            });

            modelBuilder.Entity<Title>(e =>
            {
                e.HasKey(e => e.TitleId);

                e.Property(e => e.Shortcut).HasMaxLength(16);

                e.Property(e => e.Name).HasMaxLength(64);

                e.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Role>(e =>
            {
                e.HasKey(e => e.RoleId);

                e.Property(e => e.Name).HasMaxLength(32);

                e.Property(e => e.IsActive).HasDefaultValue(true);

                e.Property(e => e.Description).HasMaxLength(256);
            });

            modelBuilder.Entity<Permission>(e =>
            {
                e.HasKey(e => e.PermissionId);

                e.Property(e => e.Name).HasMaxLength(32);

                e.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<RolePermission>(e =>
            {
                e.HasKey(e => new { e.PermissionId, e.RoleId });

                e.HasOne(e => e.Permissions).WithMany(e => e.RolePermissions).HasForeignKey(e => e.PermissionId);

                e.HasOne(e => e.Roles).WithMany(e => e.RolePermissions).HasForeignKey(e => e.RoleId);
            });

            modelBuilder.Entity<Test>(e =>
            {
                e.HasKey(e => e.TestId);
                e.Property(e => e.IsActive).HasDefaultValue(true);

                e.HasOne(e => e.Creator).WithMany().HasForeignKey(e => e.CreatorId);
                e.HasOne(e => e.StudentField).WithMany(e => e.Tests).HasForeignKey(e => e.StudentFieldId);
            });

            modelBuilder.Entity<StudentTest>(e=>
            {
                e.HasKey(e => new { e.StudentId, e.TestId });

                e.Property(e => e.LoginId).HasMaxLength(32).IsRequired();

                e.HasOne(e => e.Student).WithMany(e => e.StudentTests).HasForeignKey(e => e.StudentId);
                e.HasOne(e => e.Test).WithMany(e => e.StudentTests).HasForeignKey(e => e.TestId);
            });

            modelBuilder.Entity<Student>(e =>
            {
                e.HasKey(e => e.StudentId);
                e.Property(e => e.FirstName).HasMaxLength(64);
                e.Property(e => e.LastName).HasMaxLength(64);
                e.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<ClassesStudent>(e =>
            {
                e.HasKey(e => new { e.ClassesId, e.StudentId });
                e.HasOne(e => e.Classes).WithMany(e => e.ClassesStudents).HasForeignKey(e => e.ClassesId);
                e.HasOne(e => e.Student).WithMany(e => e.ClassesStudents).HasForeignKey(e => e.StudentId);
            });

            modelBuilder.Entity<Classes>(e =>
            {
                e.HasKey(e => e.ClassesId);
                e.Property(e => e.Name).HasMaxLength(16);
                e.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<QuestionType>(e =>
            {
                e.HasKey(e => e.QuestionTypeId);
                e.Property(e => e.Name).HasMaxLength(32);
                e.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<StudentField>(e =>
            {
                e.HasKey(e => e.StudentFieldId);
                e.Property(e => e.Name).HasMaxLength(64);
                e.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Question>(e =>
            {
                e.HasKey(e => e.QuestionId);
                e.Property(e => e.Header).HasMaxLength(128);
                e.Property(e => e.Description).HasMaxLength(512);
                e.Property(e => e.IsActive).HasDefaultValue(true);

                e.HasOne(e => e.QuestionType).WithMany(e => e.Questions).HasForeignKey(e => e.QuestionTypeId);
                e.HasOne(e => e.Field).WithMany(e => e.Questions).HasForeignKey(e => e.FieldId);
            });

            modelBuilder.Entity<QuestionOption>(e =>
            {
                e.HasKey(e => e.QuestionOptionId);
                e.Property(e => e.Text).HasMaxLength(512);
                e.Property(e => e.IsCorrect);
                e.Property(e => e.ImageKey).HasMaxLength(64);
                e.HasOne(e => e.Question).WithMany(e => e.QuestionOptions).HasForeignKey(e => e.QuestionId);
            });

            modelBuilder.Entity<TeacherRole>(e =>
            {
                e.HasKey(e => new { e.TeacherId, e.RoleId });
                e.HasOne(e => e.Teacher).WithMany(e => e.TeacherRoles).HasForeignKey(e => e.TeacherId);
                e.HasOne(e => e.Role).WithMany(e => e.TeacherRoles).HasForeignKey(e => e.RoleId);
            });

            modelBuilder.Entity<ClassesFields>(e =>
            {
                e.HasKey(e => new { e.ClassesId, e.StudentFieldId });
                e.HasOne(e => e.Classes).WithMany(e => e.ClassesFields).HasForeignKey(e => e.ClassesId);
                e.HasOne(e => e.StudentField).WithMany(e => e.ClassesFields).HasForeignKey(e => e.StudentFieldId);
            });

            modelBuilder.Entity<QuestionRow>(e =>
            {
                e.HasNoKey();
                e.ToView("QuestionRow");
            });

            modelBuilder.Entity<AuditLog>(e =>
            {
                e.HasKey(e => e.AuditLogId);
                e.Property(e => e.TeacherName).HasMaxLength(128);
                e.Property(e => e.Action).HasMaxLength(64);
                e.Property(e => e.Entity).HasMaxLength(64);
                e.Property(e => e.EntityId).HasMaxLength(64);
                e.Property(e => e.Detail).HasMaxLength(1024);
                e.HasIndex(e => e.CreatedAt);
            });


            // ========================================
            // SEED DATA — testovací data SPŠUL
            // ========================================
            // Hesla: Admin→admin1234, ostatní→heslo1234
            var hesloHash = "$2b$10$4l/ga1u8GL4dxznTb/t73eiKqRRfMKIsLpi8bCQQxkGtmnEX64NoS";

            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { TeacherId = 1, FirstName = "Admin", LastName = "Admin", NickName = "Admin", PasswordHash = "$2b$10$jErDDvlTESkhHfdiHuRFte9ojuRZNZST.gskJ4PVgp6h6q0VGmVxS" },
                new Teacher { TeacherId = 2, FirstName = "Filip", LastName = "Eder", NickName = "FilipEder", PasswordHash = hesloHash },
                new Teacher { TeacherId = 3, FirstName = "Petr", LastName = "Novák", NickName = "PetrNovak", PasswordHash = "$2b$10$YqPz7WEHhmjRpRuFqaVPVu505tO1z4KwGVnnj3T3J0S9SEnZMrZSG" },
                new Teacher { TeacherId = 4, FirstName = "Jana", LastName = "Svobodová", NickName = "JanaSvobodova", PasswordHash = hesloHash },
                new Teacher { TeacherId = 5, FirstName = "Tomáš", LastName = "Dvořák", NickName = "TomasDvorak", PasswordHash = hesloHash },
                new Teacher { TeacherId = 6, FirstName = "Marie", LastName = "Černá", NickName = "MarieCerna", PasswordHash = hesloHash },
                new Teacher { TeacherId = 7, FirstName = "Jan", LastName = "Procházka", NickName = "JanProchazka", PasswordHash = hesloHash },
                new Teacher { TeacherId = 8, FirstName = "Eva", LastName = "Krejčí", NickName = "EvaKrejci", PasswordHash = hesloHash, IsActive = false } // neaktivní učitel
            );

            modelBuilder.Entity<TeacherTitle>().HasData(
                new TeacherTitle { TeacherId = 4, TitleId = 2 },  // Mgr. Svobodová
                new TeacherTitle { TeacherId = 5, TitleId = 3 },  // Ing. Dvořák
                new TeacherTitle { TeacherId = 6, TitleId = 1 },  // Bc. Černá
                new TeacherTitle { TeacherId = 7, TitleId = 3 },  // Ing. Procházka
                new TeacherTitle { TeacherId = 7, TitleId = 7 },  // Ing. Procházka, Ph.D.
                new TeacherTitle { TeacherId = 8, TitleId = 2 }   // Mgr. Krejčí
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "Administrátor", Description = "Oprávnění uděluje plnou kontrolu nad systémem – správce může vytvářet, upravovat i mazat všechny účty.", IsActive = true },
                new Role { RoleId = 2, Name = "Tvůrce", Description = "Oprávnění uděluje možnost vytváření, aktualizování a čtení všech systému v aplikaci, krom učitelů.", IsActive = true },
                new Role { RoleId = 3, Name = "Testátor", Description = "Oprávnění uděluje možnost všechny operace pro systém testů.", IsActive = true },
                new Role { RoleId = 4, Name = "Učitelátor", Description = "Oprávnění uděluje možnost všechny operace pro systém učitelů.", IsActive = true },
                new Role { RoleId = 5, Name = "Studentátor", Description = "Oprávnění uděluje možnost všechny operace pro systém studentů.", IsActive = true },
                new Role { RoleId = 6, Name = "Hledič", Description = "Oprávnění uděluje pohled na všechny systémy.", IsActive = true }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionId = 1, Name = "All Permissions", IsActive = true },
                new Permission { PermissionId = 2, Name = "CURD", IsActive = true },
                new Permission { PermissionId = 3, Name = "CRUD Test", IsActive = true },
                new Permission { PermissionId = 4, Name = "CRUD Teacher", IsActive = true },
                new Permission { PermissionId = 5, Name = "CRUD Student", IsActive = true },
                new Permission { PermissionId = 6, Name = "View", IsActive = true }
            );

            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { RoleId = 1, PermissionId = 1 },
                new RolePermission { RoleId = 2, PermissionId = 2 },
                new RolePermission { RoleId = 3, PermissionId = 3 },
                new RolePermission { RoleId = 4, PermissionId = 4 },
                new RolePermission { RoleId = 5, PermissionId = 5 },
                new RolePermission { RoleId = 6, PermissionId = 6 }
            );

            modelBuilder.Entity<TeacherRole>().HasData(
                new TeacherRole { TeacherId = 1, RoleId = 1 },  // Admin → Administrátor
                new TeacherRole { TeacherId = 2, RoleId = 6 },  // Eder → Hledič
                new TeacherRole { TeacherId = 4, RoleId = 2 },  // Svobodová → Tvůrce
                new TeacherRole { TeacherId = 5, RoleId = 3 },  // Dvořák → Testátor
                new TeacherRole { TeacherId = 6, RoleId = 5 },  // Černá → Studentátor
                new TeacherRole { TeacherId = 7, RoleId = 2 },  // Procházka → Tvůrce
                new TeacherRole { TeacherId = 8, RoleId = 4 }   // Krejčí → Učitelátor
            );

            modelBuilder.Entity<Title>().HasData(
                new Title { TitleId = 1, Shortcut = "Bc.", Name = "Bakalář", IsActive = true },
                new Title { TitleId = 2, Shortcut = "Mgr.", Name = "Magistr", IsActive = true },
                new Title { TitleId = 3, Shortcut = "Ing.", Name = "Inženýr", IsActive = true },
                new Title { TitleId = 4, Shortcut = "PhDr.", Name = "Doktor filozofie", IsActive = true },
                new Title { TitleId = 5, Shortcut = "JUDr.", Name = "Doktor práv", IsActive = true },
                new Title { TitleId = 6, Shortcut = "RNDr.", Name = "Doktor přírodních věd", IsActive = true },
                new Title { TitleId = 7, Shortcut = "Ph.D.", Name = "Doktor filozofie", IsActive = true },
                new Title { TitleId = 8, Shortcut = "Th.D.", Name = "Doktor teologie", IsActive = true },
                new Title { TitleId = 9, Shortcut = "MBA", Name = "Magisterský titul obchodní administrativy", IsActive = true },
                new Title { TitleId = 10, Shortcut = "LL.M.", Name = "Magistr práv", IsActive = true }
            );

            modelBuilder.Entity<StudentField>().HasData(
                new StudentField { StudentFieldId = 1, Name = "Anglický jazyk", IsActive = true },
                new StudentField { StudentFieldId = 2, Name = "Databáze", IsActive = true },
                new StudentField { StudentFieldId = 3, Name = "Ekonomika", IsActive = true },
                new StudentField { StudentFieldId = 4, Name = "Elektrotechnika", IsActive = true },
                new StudentField { StudentFieldId = 5, Name = "Fyzika", IsActive = true },
                new StudentField { StudentFieldId = 6, Name = "Kyberbezpečnost", IsActive = true },
                new StudentField { StudentFieldId = 7, Name = "Matematický seminář", IsActive = true },
                new StudentField { StudentFieldId = 8, Name = "Matematika", IsActive = true },
                new StudentField { StudentFieldId = 9, Name = "Operační systémy", IsActive = true },
                new StudentField { StudentFieldId = 10, Name = "Praxe", IsActive = true },
                new StudentField { StudentFieldId = 11, Name = "Programování a vývoj aplikací", IsActive = true },
                new StudentField { StudentFieldId = 12, Name = "Projekty", IsActive = true },
                new StudentField { StudentFieldId = 13, Name = "Tělesná výchova", IsActive = true },
                new StudentField { StudentFieldId = 14, Name = "Český jazyk a literatura", IsActive = true },
                new StudentField { StudentFieldId = 15, Name = "Základy elektrotechniky", IsActive = true },
                new StudentField { StudentFieldId = 16, Name = "Materiály a technologie", IsActive = true },
                new StudentField { StudentFieldId = 17, Name = "Informační a komunikační technologie", IsActive = true },
                new StudentField { StudentFieldId = 18, Name = "Nauka o společnosti", IsActive = true },
                new StudentField { StudentFieldId = 19, Name = "Odborný výcvik", IsActive = true },
                new StudentField { StudentFieldId = 20, Name = "Elektrické stroje a přístroje", IsActive = true },
                new StudentField { StudentFieldId = 21, Name = "Technická dokumentace", IsActive = true },
                new StudentField { StudentFieldId = 22, Name = "Dějepis", IsActive = true },
                new StudentField { StudentFieldId = 23, Name = "Aplikační software", IsActive = true },
                new StudentField { StudentFieldId = 24, Name = "Webové aplikace", IsActive = true },
                new StudentField { StudentFieldId = 25, Name = "Základy přírodních věd", IsActive = true },
                new StudentField { StudentFieldId = 26, Name = "Algoritmizace", IsActive = true },
                new StudentField { StudentFieldId = 27, Name = "Datové sítě", IsActive = true },
                new StudentField { StudentFieldId = 28, Name = "Logistika", IsActive = true },
                new StudentField { StudentFieldId = 29, Name = "Doprava", IsActive = true },
                new StudentField { StudentFieldId = 30, Name = "Německý jazyk", IsActive = true },
                new StudentField { StudentFieldId = 31, Name = "Písemná a elektronická komunikace", IsActive = true },
                new StudentField { StudentFieldId = 32, Name = "Občanská nauka", IsActive = true },
                new StudentField { StudentFieldId = 33, Name = "Automatizace", IsActive = true },
                new StudentField { StudentFieldId = 34, Name = "Elektronika", IsActive = true },
                new StudentField { StudentFieldId = 35, Name = "Elektrotechnická měření", IsActive = true },
                new StudentField { StudentFieldId = 36, Name = "Herní vývoj", IsActive = true },
                new StudentField { StudentFieldId = 37, Name = "Marketing a management", IsActive = true },
                new StudentField { StudentFieldId = 38, Name = "Účetnictví", IsActive = true },
                new StudentField { StudentFieldId = 39, Name = "Webové technologie", IsActive = true },
                new StudentField { StudentFieldId = 40, Name = "Zeměpis", IsActive = true },
                new StudentField { StudentFieldId = 41, Name = "Strojnictví", IsActive = true },
                new StudentField { StudentFieldId = 42, Name = "Programování", IsActive = true },
                new StudentField { StudentFieldId = 43, Name = "Elektronika a sdělovací technika", IsActive = true },
                new StudentField { StudentFieldId = 44, Name = "Číslicová technika", IsActive = true },
                new StudentField { StudentFieldId = 45, Name = "Automatizace", IsActive = true },
                new StudentField { StudentFieldId = 46, Name = "Mikroprocesorová technika", IsActive = true },
                new StudentField { StudentFieldId = 47, Name = "Technické kreslení", IsActive = true }
            );

            modelBuilder.Entity<QuestionType>().HasData(
                new QuestionType { QuestionTypeId = 1, Name = "Výběr z možností", IsActive = true },
                new QuestionType { QuestionTypeId = 2, Name = "Uzavřená otázka s obrázky", IsActive = true }
            );

            // ── Třídy ──
            modelBuilder.Entity<Classes>().HasData(
                new Classes { ClassesId = 1, Name = "1.A", StartFrom = 2025, EndTo = 2029, IsActive = true },
                new Classes { ClassesId = 2, Name = "1.B", StartFrom = 2025, EndTo = 2029, IsActive = true },
                new Classes { ClassesId = 3, Name = "2.A", StartFrom = 2024, EndTo = 2028, IsActive = true },
                new Classes { ClassesId = 4, Name = "2.B", StartFrom = 2024, EndTo = 2028, IsActive = true },
                new Classes { ClassesId = 5, Name = "3.A", StartFrom = 2023, EndTo = 2027, IsActive = true },
                new Classes { ClassesId = 6, Name = "4.A", StartFrom = 2022, EndTo = 2026, IsActive = false } // absolventi
            );

            // ── Předměty přiřazené třídám ──
            modelBuilder.Entity<ClassesFields>().HasData(
                // 1.A (IT): PVA, Algoritmizace, Matematika, Angličtina, ICT
                new ClassesFields { ClassesId = 1, StudentFieldId = 11 },
                new ClassesFields { ClassesId = 1, StudentFieldId = 26 },
                new ClassesFields { ClassesId = 1, StudentFieldId = 8 },
                new ClassesFields { ClassesId = 1, StudentFieldId = 1 },
                new ClassesFields { ClassesId = 1, StudentFieldId = 17 },
                // 1.B (Elektro): Elektrotechnika, ZE, Matematika, Fyzika, Angličtina
                new ClassesFields { ClassesId = 2, StudentFieldId = 4 },
                new ClassesFields { ClassesId = 2, StudentFieldId = 15 },
                new ClassesFields { ClassesId = 2, StudentFieldId = 8 },
                new ClassesFields { ClassesId = 2, StudentFieldId = 5 },
                new ClassesFields { ClassesId = 2, StudentFieldId = 1 },
                // 2.A (IT): Databáze, PVA, Webové aplikace, OS, Matematika
                new ClassesFields { ClassesId = 3, StudentFieldId = 2 },
                new ClassesFields { ClassesId = 3, StudentFieldId = 11 },
                new ClassesFields { ClassesId = 3, StudentFieldId = 24 },
                new ClassesFields { ClassesId = 3, StudentFieldId = 9 },
                new ClassesFields { ClassesId = 3, StudentFieldId = 8 },
                // 2.B (Elektro): Elektronika, Automatizace, Elektrotechnika, Matematika
                new ClassesFields { ClassesId = 4, StudentFieldId = 34 },
                new ClassesFields { ClassesId = 4, StudentFieldId = 33 },
                new ClassesFields { ClassesId = 4, StudentFieldId = 4 },
                new ClassesFields { ClassesId = 4, StudentFieldId = 8 },
                // 3.A (IT): Kyberbezpečnost, Datové sítě, Programování, Projekty
                new ClassesFields { ClassesId = 5, StudentFieldId = 6 },
                new ClassesFields { ClassesId = 5, StudentFieldId = 27 },
                new ClassesFields { ClassesId = 5, StudentFieldId = 42 },
                new ClassesFields { ClassesId = 5, StudentFieldId = 12 },
                // 4.A (IT): Projekty, Praxe, Programování, Webové technologie
                new ClassesFields { ClassesId = 6, StudentFieldId = 12 },
                new ClassesFields { ClassesId = 6, StudentFieldId = 10 },
                new ClassesFields { ClassesId = 6, StudentFieldId = 42 },
                new ClassesFields { ClassesId = 6, StudentFieldId = 39 }
            );

            // ── Studenti (24 studentů, 4 na třídu) ──
            modelBuilder.Entity<Student>().HasData(
                // 1.A
                new Student { StudentId = 1, FirstName = "Jakub", LastName = "Horák", IsActive = true },
                new Student { StudentId = 2, FirstName = "Tereza", LastName = "Marková", IsActive = true },
                new Student { StudentId = 3, FirstName = "Martin", LastName = "Jelínek", IsActive = true },
                new Student { StudentId = 4, FirstName = "Lucie", LastName = "Pokorná", IsActive = true },
                // 1.B
                new Student { StudentId = 5, FirstName = "David", LastName = "Růžička", IsActive = true },
                new Student { StudentId = 6, FirstName = "Anna", LastName = "Benešová", IsActive = true },
                new Student { StudentId = 7, FirstName = "Ondřej", LastName = "Fiala", IsActive = true },
                new Student { StudentId = 8, FirstName = "Karolína", LastName = "Šťastná", IsActive = true },
                // 2.A
                new Student { StudentId = 9, FirstName = "Vojtěch", LastName = "Kučera", IsActive = true },
                new Student { StudentId = 10, FirstName = "Eliška", LastName = "Veselá", IsActive = true },
                new Student { StudentId = 11, FirstName = "Matěj", LastName = "Marek", IsActive = true },
                new Student { StudentId = 12, FirstName = "Natálie", LastName = "Kopecká", IsActive = false }, // přestup
                // 2.B
                new Student { StudentId = 13, FirstName = "Štěpán", LastName = "Havlíček", IsActive = true },
                new Student { StudentId = 14, FirstName = "Michaela", LastName = "Vlčková", IsActive = true },
                new Student { StudentId = 15, FirstName = "Adam", LastName = "Bartoš", IsActive = true },
                new Student { StudentId = 16, FirstName = "Barbora", LastName = "Urbanová", IsActive = true },
                // 3.A
                new Student { StudentId = 17, FirstName = "Daniel", LastName = "Blažek", IsActive = true },
                new Student { StudentId = 18, FirstName = "Kristýna", LastName = "Sedláčková", IsActive = true },
                new Student { StudentId = 19, FirstName = "Filip", LastName = "Kratochvíl", IsActive = true },
                new Student { StudentId = 20, FirstName = "Simona", LastName = "Němcová", IsActive = true },
                // 4.A
                new Student { StudentId = 21, FirstName = "Lukáš", LastName = "Pospíšil", IsActive = true },
                new Student { StudentId = 22, FirstName = "Veronika", LastName = "Holubová", IsActive = true },
                new Student { StudentId = 23, FirstName = "Dominik", LastName = "Šimek", IsActive = true },
                new Student { StudentId = 24, FirstName = "Klára", LastName = "Dostálová", IsActive = true }
            );

            modelBuilder.Entity<ClassesStudent>().HasData(
                // 1.A
                new ClassesStudent { ClassesId = 1, StudentId = 1 },
                new ClassesStudent { ClassesId = 1, StudentId = 2 },
                new ClassesStudent { ClassesId = 1, StudentId = 3 },
                new ClassesStudent { ClassesId = 1, StudentId = 4 },
                // 1.B
                new ClassesStudent { ClassesId = 2, StudentId = 5 },
                new ClassesStudent { ClassesId = 2, StudentId = 6 },
                new ClassesStudent { ClassesId = 2, StudentId = 7 },
                new ClassesStudent { ClassesId = 2, StudentId = 8 },
                // 2.A
                new ClassesStudent { ClassesId = 3, StudentId = 9 },
                new ClassesStudent { ClassesId = 3, StudentId = 10 },
                new ClassesStudent { ClassesId = 3, StudentId = 11 },
                new ClassesStudent { ClassesId = 3, StudentId = 12 },
                // 2.B
                new ClassesStudent { ClassesId = 4, StudentId = 13 },
                new ClassesStudent { ClassesId = 4, StudentId = 14 },
                new ClassesStudent { ClassesId = 4, StudentId = 15 },
                new ClassesStudent { ClassesId = 4, StudentId = 16 },
                // 3.A
                new ClassesStudent { ClassesId = 5, StudentId = 17 },
                new ClassesStudent { ClassesId = 5, StudentId = 18 },
                new ClassesStudent { ClassesId = 5, StudentId = 19 },
                new ClassesStudent { ClassesId = 5, StudentId = 20 },
                // 4.A
                new ClassesStudent { ClassesId = 6, StudentId = 21 },
                new ClassesStudent { ClassesId = 6, StudentId = 22 },
                new ClassesStudent { ClassesId = 6, StudentId = 23 },
                new ClassesStudent { ClassesId = 6, StudentId = 24 }
            );

            // ── Otázky (12 otázek, různé předměty a tvůrci) ──
            modelBuilder.Entity<Question>().HasData(
                // Programování (FieldId=11, Creator=4 Svobodová)
                new Question { QuestionId = 1, Header = "Co je to proměnná?", Description = "Základní koncept programování — vysvětlete, co znamená pojem proměnná.", QuestionTypeId = 1, FieldId = 11, CreatorId = 4, IsActive = true },
                new Question { QuestionId = 2, Header = "Který datový typ je celočíselný v C#?", Description = "Datové typy v jazyce C# — vyberte správný celočíselný typ.", QuestionTypeId = 1, FieldId = 11, CreatorId = 4, IsActive = true },
                new Question { QuestionId = 3, Header = "Co dělá příkaz 'if'?", Description = "Řídicí struktury v programování — podmíněné větvení.", QuestionTypeId = 1, FieldId = 11, CreatorId = 4, IsActive = true },
                new Question { QuestionId = 4, Header = "Co je to cyklus 'for'?", Description = "Cykly v programování — opakování bloku kódu.", QuestionTypeId = 1, FieldId = 11, CreatorId = 7, IsActive = true },
                // Databáze (FieldId=2, Creator=7 Procházka)
                new Question { QuestionId = 5, Header = "Co znamená zkratka SQL?", Description = "Základy databázových jazyků — původ zkratky SQL.", QuestionTypeId = 1, FieldId = 2, CreatorId = 7, IsActive = true },
                new Question { QuestionId = 6, Header = "Který příkaz slouží k výběru dat?", Description = "SQL příkazy pro čtení dat z databáze.", QuestionTypeId = 1, FieldId = 2, CreatorId = 7, IsActive = true },
                new Question { QuestionId = 7, Header = "Co je primární klíč?", Description = "Integritní omezení v relačních databázích.", QuestionTypeId = 1, FieldId = 2, CreatorId = 7, IsActive = true },
                new Question { QuestionId = 8, Header = "Co dělá příkaz JOIN?", Description = "Spojování tabulek v SQL dotazech.", QuestionTypeId = 1, FieldId = 2, CreatorId = 4, IsActive = true },
                // Kyberbezpečnost (FieldId=6, Creator=4 Svobodová)
                new Question { QuestionId = 9, Header = "Co je phishing?", Description = "Sociální inženýrství — rozpoznávání phishingových útoků.", QuestionTypeId = 1, FieldId = 6, CreatorId = 4, IsActive = true },
                new Question { QuestionId = 10, Header = "Co je to firewall?", Description = "Síťová bezpečnost — ochrana perimetru sítě.", QuestionTypeId = 1, FieldId = 6, CreatorId = 4, IsActive = true },
                // Matematika (FieldId=8, Creator=1 Admin)
                new Question { QuestionId = 11, Header = "Kolik je 2³?", Description = "Mocniny — výpočet třetí mocniny čísla 2.", QuestionTypeId = 1, FieldId = 8, CreatorId = 1, IsActive = true },
                // Neaktivní otázka pro testování filtrů
                new Question { QuestionId = 12, Header = "Zastaralá otázka", Description = "Tato otázka byla deaktivována a neměla by se zobrazovat v testech.", QuestionTypeId = 1, FieldId = 11, CreatorId = 4, IsActive = false }
            );

            // ── Možnosti odpovědí (4 na otázku, 1 správná) ──
            modelBuilder.Entity<QuestionOption>().HasData(
                // Q1: Co je to proměnná?
                new QuestionOption { QuestionOptionId = 1, QuestionId = 1, Text = "Funkce pro výpočet", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 2, QuestionId = 1, Text = "Pojmenované místo v paměti pro uložení hodnoty", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 3, QuestionId = 1, Text = "Typ souboru", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 4, QuestionId = 1, Text = "Příkaz pro výstup na obrazovku", IsCorrect = false, ImageKey = "" },
                // Q2: Celočíselný typ v C#?
                new QuestionOption { QuestionOptionId = 5, QuestionId = 2, Text = "number", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 6, QuestionId = 2, Text = "text", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 7, QuestionId = 2, Text = "int", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 8, QuestionId = 2, Text = "letra", IsCorrect = false, ImageKey = "" },
                // Q3: Co dělá if?
                new QuestionOption { QuestionOptionId = 9, QuestionId = 3, Text = "Opakuje blok kódu", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 10, QuestionId = 3, Text = "Definuje novou funkci", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 11, QuestionId = 3, Text = "Vykoná kód pouze pokud je podmínka pravdivá", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 12, QuestionId = 3, Text = "Ukončí program", IsCorrect = false, ImageKey = "" },
                // Q4: Co je cyklus for?
                new QuestionOption { QuestionOptionId = 13, QuestionId = 4, Text = "Podmíněný příkaz", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 14, QuestionId = 4, Text = "Cyklus s předem daným počtem opakování", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 15, QuestionId = 4, Text = "Deklarace proměnné", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 16, QuestionId = 4, Text = "Import knihovny", IsCorrect = false, ImageKey = "" },
                // Q5: Co znamená SQL?
                new QuestionOption { QuestionOptionId = 17, QuestionId = 5, Text = "Standard Query Language", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 18, QuestionId = 5, Text = "Structured Query Language", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 19, QuestionId = 5, Text = "System Query Logic", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 20, QuestionId = 5, Text = "Simple Question Language", IsCorrect = false, ImageKey = "" },
                // Q6: Příkaz pro výběr dat?
                new QuestionOption { QuestionOptionId = 21, QuestionId = 6, Text = "INSERT", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 22, QuestionId = 6, Text = "UPDATE", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 23, QuestionId = 6, Text = "SELECT", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 24, QuestionId = 6, Text = "DELETE", IsCorrect = false, ImageKey = "" },
                // Q7: Co je primární klíč?
                new QuestionOption { QuestionOptionId = 25, QuestionId = 7, Text = "Heslo do databáze", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 26, QuestionId = 7, Text = "Unikátní identifikátor záznamu v tabulce", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 27, QuestionId = 7, Text = "Název tabulky", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 28, QuestionId = 7, Text = "Typ sloupce", IsCorrect = false, ImageKey = "" },
                // Q8: Co dělá JOIN?
                new QuestionOption { QuestionOptionId = 29, QuestionId = 8, Text = "Maže data ze dvou tabulek", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 30, QuestionId = 8, Text = "Spojuje data z více tabulek na základě podmínky", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 31, QuestionId = 8, Text = "Vytváří novou tabulku", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 32, QuestionId = 8, Text = "Řadí výsledky vzestupně", IsCorrect = false, ImageKey = "" },
                // Q9: Co je phishing?
                new QuestionOption { QuestionOptionId = 33, QuestionId = 9, Text = "Antivirový program", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 34, QuestionId = 9, Text = "Podvodný pokus o získání citlivých údajů", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 35, QuestionId = 9, Text = "Šifrovací algoritmus", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 36, QuestionId = 9, Text = "Typ síťového kabelu", IsCorrect = false, ImageKey = "" },
                // Q10: Co je firewall?
                new QuestionOption { QuestionOptionId = 37, QuestionId = 10, Text = "Hardware pro tisk", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 38, QuestionId = 10, Text = "Zařízení/software filtrující síťový provoz", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 39, QuestionId = 10, Text = "Typ operačního systému", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 40, QuestionId = 10, Text = "Programovací jazyk", IsCorrect = false, ImageKey = "" },
                // Q11: Kolik je 2³?
                new QuestionOption { QuestionOptionId = 41, QuestionId = 11, Text = "6", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 42, QuestionId = 11, Text = "8", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 43, QuestionId = 11, Text = "9", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 44, QuestionId = 11, Text = "4", IsCorrect = false, ImageKey = "" },
                // Q12: Zastaralá (neaktivní)
                new QuestionOption { QuestionOptionId = 45, QuestionId = 12, Text = "Odpověď A", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 46, QuestionId = 12, Text = "Odpověď B", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 47, QuestionId = 12, Text = "Odpověď C", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 48, QuestionId = 12, Text = "Odpověď D", IsCorrect = false, ImageKey = "" }
            );

            // ── Testy (3 testy s JSON snapshoty otázek) ──
            modelBuilder.Entity<Test>().HasData(
                new Test
                {
                    TestId = 1,
                    Name = "PVA — Základy programování",
                    CreatorId = 4, // Svobodová
                    StudentFieldId = 11,
                    TimeLimitMinutes = 15,
                    ShuffleQuestions = true,
                    IsActive = true,
                    QuestionSnapshot = """[{"QuestionId":1,"Header":"Co je to proměnná?","Description":"Základní koncept programování.","QuestionType":"Výběr z možností","Options":[{"Text":"Funkce pro výpočet","ImageKey":null,"IsCorrect":false},{"Text":"Pojmenované místo v paměti pro uložení hodnoty","ImageKey":null,"IsCorrect":true},{"Text":"Typ souboru","ImageKey":null,"IsCorrect":false},{"Text":"Příkaz pro výstup na obrazovku","ImageKey":null,"IsCorrect":false}]},{"QuestionId":2,"Header":"Který datový typ je celočíselný v C#?","Description":"Datové typy v jazyce C#.","QuestionType":"Výběr z možností","Options":[{"Text":"number","ImageKey":null,"IsCorrect":false},{"Text":"text","ImageKey":null,"IsCorrect":false},{"Text":"int","ImageKey":null,"IsCorrect":true},{"Text":"letra","ImageKey":null,"IsCorrect":false}]},{"QuestionId":3,"Header":"Co dělá příkaz 'if'?","Description":"Řídicí struktury.","QuestionType":"Výběr z možností","Options":[{"Text":"Opakuje blok kódu","ImageKey":null,"IsCorrect":false},{"Text":"Definuje novou funkci","ImageKey":null,"IsCorrect":false},{"Text":"Vykoná kód pouze pokud je podmínka pravdivá","ImageKey":null,"IsCorrect":true},{"Text":"Ukončí program","ImageKey":null,"IsCorrect":false}]},{"QuestionId":4,"Header":"Co je to cyklus 'for'?","Description":"Cykly v programování.","QuestionType":"Výběr z možností","Options":[{"Text":"Podmíněný příkaz","ImageKey":null,"IsCorrect":false},{"Text":"Cyklus s předem daným počtem opakování","ImageKey":null,"IsCorrect":true},{"Text":"Deklarace proměnné","ImageKey":null,"IsCorrect":false},{"Text":"Import knihovny","ImageKey":null,"IsCorrect":false}]}]"""
                },
                new Test
                {
                    TestId = 2,
                    Name = "Databáze — SQL základy",
                    CreatorId = 7, // Procházka
                    StudentFieldId = 2,
                    TimeLimitMinutes = 20,
                    ShuffleQuestions = false,
                    IsActive = true,
                    QuestionSnapshot = """[{"QuestionId":5,"Header":"Co znamená zkratka SQL?","Description":"Základy databázových jazyků.","QuestionType":"Výběr z možností","Options":[{"Text":"Standard Query Language","ImageKey":null,"IsCorrect":false},{"Text":"Structured Query Language","ImageKey":null,"IsCorrect":true},{"Text":"System Query Logic","ImageKey":null,"IsCorrect":false},{"Text":"Simple Question Language","ImageKey":null,"IsCorrect":false}]},{"QuestionId":6,"Header":"Který příkaz slouží k výběru dat?","Description":"SQL příkazy.","QuestionType":"Výběr z možností","Options":[{"Text":"INSERT","ImageKey":null,"IsCorrect":false},{"Text":"UPDATE","ImageKey":null,"IsCorrect":false},{"Text":"SELECT","ImageKey":null,"IsCorrect":true},{"Text":"DELETE","ImageKey":null,"IsCorrect":false}]},{"QuestionId":7,"Header":"Co je primární klíč?","Description":"Integritní omezení.","QuestionType":"Výběr z možností","Options":[{"Text":"Heslo do databáze","ImageKey":null,"IsCorrect":false},{"Text":"Unikátní identifikátor záznamu v tabulce","ImageKey":null,"IsCorrect":true},{"Text":"Název tabulky","ImageKey":null,"IsCorrect":false},{"Text":"Typ sloupce","ImageKey":null,"IsCorrect":false}]},{"QuestionId":8,"Header":"Co dělá příkaz JOIN?","Description":"Spojování tabulek.","QuestionType":"Výběr z možností","Options":[{"Text":"Maže data ze dvou tabulek","ImageKey":null,"IsCorrect":false},{"Text":"Spojuje data z více tabulek na základě podmínky","ImageKey":null,"IsCorrect":true},{"Text":"Vytváří novou tabulku","ImageKey":null,"IsCorrect":false},{"Text":"Řadí výsledky vzestupně","ImageKey":null,"IsCorrect":false}]}]"""
                },
                new Test
                {
                    TestId = 3,
                    Name = "Kyberbezpečnost — Úvod",
                    CreatorId = 4, // Svobodová
                    StudentFieldId = 6,
                    TimeLimitMinutes = null, // bez limitu
                    ShuffleQuestions = true,
                    IsActive = false, // uzavřený test
                    QuestionSnapshot = """[{"QuestionId":9,"Header":"Co je phishing?","Description":"Sociální inženýrství.","QuestionType":"Výběr z možností","Options":[{"Text":"Antivirový program","ImageKey":null,"IsCorrect":false},{"Text":"Podvodný pokus o získání citlivých údajů","ImageKey":null,"IsCorrect":true},{"Text":"Šifrovací algoritmus","ImageKey":null,"IsCorrect":false},{"Text":"Typ síťového kabelu","ImageKey":null,"IsCorrect":false}]},{"QuestionId":10,"Header":"Co je to firewall?","Description":"Síťová bezpečnost.","QuestionType":"Výběr z možností","Options":[{"Text":"Hardware pro tisk","ImageKey":null,"IsCorrect":false},{"Text":"Zařízení/software filtrující síťový provoz","ImageKey":null,"IsCorrect":true},{"Text":"Typ operačního systému","ImageKey":null,"IsCorrect":false},{"Text":"Programovací jazyk","ImageKey":null,"IsCorrect":false}]}]"""
                }
            );

            // ── Přiřazení testů studentům ──
            modelBuilder.Entity<StudentTest>().HasData(
                // Test 1 (PVA) → studenti z 1.A — Horák dokončil, Marková rozepsaná, Jelínek nezačal
                new StudentTest
                {
                    StudentId = 1, TestId = 1, LoginId = "PVA-HOR-001",
                    StartedAt = new DateTime(2026, 3, 10, 8, 0, 0, DateTimeKind.Utc),
                    FinishedAt = new DateTime(2026, 3, 10, 8, 12, 0, DateTimeKind.Utc),
                    ResultSnapshot = """{"Answers":[{"QuestionId":1,"SelectedOptions":["Pojmenované místo v paměti pro uložení hodnoty"]},{"QuestionId":2,"SelectedOptions":["int"]},{"QuestionId":3,"SelectedOptions":["Vykoná kód pouze pokud je podmínka pravdivá"]},{"QuestionId":4,"SelectedOptions":["Cyklus s předem daným počtem opakování"]}],"CurrentQuestionIndex":3}"""
                },
                new StudentTest
                {
                    StudentId = 2, TestId = 1, LoginId = "PVA-MAR-002",
                    StartedAt = new DateTime(2026, 3, 10, 8, 0, 0, DateTimeKind.Utc),
                    FinishedAt = DateTime.MinValue, // rozepsaný
                    ResultSnapshot = """{"Answers":[{"QuestionId":1,"SelectedOptions":["Pojmenované místo v paměti pro uložení hodnoty"]},{"QuestionId":2,"SelectedOptions":["text"]}],"CurrentQuestionIndex":2}"""
                },
                new StudentTest
                {
                    StudentId = 3, TestId = 1, LoginId = "PVA-JEL-003",
                    StartedAt = DateTime.MinValue, // nezačal
                    FinishedAt = DateTime.MinValue,
                    ResultSnapshot = """{"Answers":[],"CurrentQuestionIndex":0}"""
                },
                // Test 2 (SQL) → studenti z 2.A — Kučera dokončil (3/4 správně), Veselá dokončila (4/4)
                new StudentTest
                {
                    StudentId = 9, TestId = 2, LoginId = "SQL-KUC-009",
                    StartedAt = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
                    FinishedAt = new DateTime(2026, 3, 12, 10, 18, 0, DateTimeKind.Utc),
                    ResultSnapshot = """{"Answers":[{"QuestionId":5,"SelectedOptions":["Structured Query Language"]},{"QuestionId":6,"SelectedOptions":["SELECT"]},{"QuestionId":7,"SelectedOptions":["Název tabulky"]},{"QuestionId":8,"SelectedOptions":["Spojuje data z více tabulek na základě podmínky"]}],"CurrentQuestionIndex":3}"""
                },
                new StudentTest
                {
                    StudentId = 10, TestId = 2, LoginId = "SQL-VES-010",
                    StartedAt = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
                    FinishedAt = new DateTime(2026, 3, 12, 10, 15, 0, DateTimeKind.Utc),
                    ResultSnapshot = """{"Answers":[{"QuestionId":5,"SelectedOptions":["Structured Query Language"]},{"QuestionId":6,"SelectedOptions":["SELECT"]},{"QuestionId":7,"SelectedOptions":["Unikátní identifikátor záznamu v tabulce"]},{"QuestionId":8,"SelectedOptions":["Spojuje data z více tabulek na základě podmínky"]}],"CurrentQuestionIndex":3}"""
                },
                new StudentTest
                {
                    StudentId = 11, TestId = 2, LoginId = "SQL-MAR-011",
                    StartedAt = DateTime.MinValue,
                    FinishedAt = DateTime.MinValue,
                    ResultSnapshot = """{"Answers":[],"CurrentQuestionIndex":0}"""
                },
                // Test 3 (Kyber) → studenti z 3.A — Blažek dokončil
                new StudentTest
                {
                    StudentId = 17, TestId = 3, LoginId = "KYB-BLA-017",
                    StartedAt = new DateTime(2026, 2, 20, 9, 0, 0, DateTimeKind.Utc),
                    FinishedAt = new DateTime(2026, 2, 20, 9, 8, 0, DateTimeKind.Utc),
                    ResultSnapshot = """{"Answers":[{"QuestionId":9,"SelectedOptions":["Podvodný pokus o získání citlivých údajů"]},{"QuestionId":10,"SelectedOptions":["Zařízení/software filtrující síťový provoz"]}],"CurrentQuestionIndex":1}"""
                },
                new StudentTest
                {
                    StudentId = 18, TestId = 3, LoginId = "KYB-SED-018",
                    StartedAt = new DateTime(2026, 2, 20, 9, 0, 0, DateTimeKind.Utc),
                    FinishedAt = new DateTime(2026, 2, 20, 9, 5, 0, DateTimeKind.Utc),
                    ResultSnapshot = """{"Answers":[{"QuestionId":9,"SelectedOptions":["Šifrovací algoritmus"]},{"QuestionId":10,"SelectedOptions":["Zařízení/software filtrující síťový provoz"]}],"CurrentQuestionIndex":1}"""
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
