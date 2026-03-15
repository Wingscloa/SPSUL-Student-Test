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
            // SEED DATA
            // ========================================
            // Hesla: Admin->admin1234, ostatni->heslo1234
            var hesloHash = "$2b$10$PMAUpgwQ3pwpaIMv1CCYA.YzbHYl5.PXqmsRNBaR1Oevp3m7w1v2y";

            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { TeacherId = 1, FirstName = "Admin", LastName = "Admin", NickName = "Admin", PasswordHash = "$2b$10$jErDDvlTESkhHfdiHuRFte9ojuRZNZST.gskJ4PVgp6h6q0VGmVxS" },
                new Teacher { TeacherId = 2, FirstName = "Filip", LastName = "Eder", NickName = "FilipEder", PasswordHash = hesloHash },
                new Teacher { TeacherId = 3, FirstName = "Petr", LastName = "Novak", NickName = "PetrNovak", PasswordHash = "$2b$10$YqPz7WEHhmjRpRuFqaVPVu505tO1z4KwGVnnj3T3J0S9SEnZMrZSG" },
                new Teacher { TeacherId = 4, FirstName = "Jana", LastName = "Svobodova", NickName = "JanaSvobodova", PasswordHash = hesloHash },
                new Teacher { TeacherId = 5, FirstName = "Tomas", LastName = "Dvorak", NickName = "TomasDvorak", PasswordHash = hesloHash },
                new Teacher { TeacherId = 6, FirstName = "Marie", LastName = "Cerna", NickName = "MarieCerna", PasswordHash = hesloHash },
                new Teacher { TeacherId = 7, FirstName = "Jan", LastName = "Prochazka", NickName = "JanProchazka", PasswordHash = hesloHash },
                new Teacher { TeacherId = 8, FirstName = "Eva", LastName = "Krejci", NickName = "EvaKrejci", PasswordHash = hesloHash, IsActive = false }
            );

            modelBuilder.Entity<TeacherTitle>().HasData(
                new TeacherTitle { TeacherId = 4, TitleId = 2 },  // Mgr. Svobodova
                new TeacherTitle { TeacherId = 5, TitleId = 3 },  // Ing. Dvorak
                new TeacherTitle { TeacherId = 6, TitleId = 1 },  // Bc. Cerna
                new TeacherTitle { TeacherId = 7, TitleId = 3 },  // Ing. Prochazka
                new TeacherTitle { TeacherId = 7, TitleId = 7 },  // Ing. Prochazka, Ph.D.
                new TeacherTitle { TeacherId = 8, TitleId = 2 }   // Mgr. Krejci
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "Administrator", Description = "Opravneni udeluje plnou kontrolu nad systemem – spravce muze vytvaret, upravovat i mazat vsechny ucty.", IsActive = true },
                new Role { RoleId = 2, Name = "Tvurce", Description = "Opravneni udeluje moznost vytvareni, aktualizovani a cteni vsech systemu v aplikaci, krom ucitelu.", IsActive = true },
                new Role { RoleId = 3, Name = "Testator", Description = "Opravneni udeluje moznost vsechny operace pro system testu.", IsActive = true },
                new Role { RoleId = 4, Name = "Ucitelator", Description = "Opravneni udeluje moznost vsechny operace pro system ucitelu.", IsActive = true },
                new Role { RoleId = 5, Name = "Studentator", Description = "Opravneni udeluje moznost vsechny operace pro system studentu.", IsActive = true },
                new Role { RoleId = 6, Name = "Hledic", Description = "Opravneni udeluje pohled na vsechny systemy.", IsActive = true }
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
                new TeacherRole { TeacherId = 1, RoleId = 1 },  // Admin -> Administrator
                new TeacherRole { TeacherId = 2, RoleId = 6 },  // Eder -> Hledic
                new TeacherRole { TeacherId = 4, RoleId = 2 },  // Svobodova -> Tvurce
                new TeacherRole { TeacherId = 5, RoleId = 3 },  // Dvorak -> Testator
                new TeacherRole { TeacherId = 6, RoleId = 5 },  // Cerna -> Studentator
                new TeacherRole { TeacherId = 7, RoleId = 2 },  // Prochazka -> Tvurce
                new TeacherRole { TeacherId = 8, RoleId = 4 }   // Krejci -> Ucitelator
            );

            modelBuilder.Entity<Title>().HasData(
                new Title { TitleId = 1, Shortcut = "Bc.", Name = "Bakalar", IsActive = true },
                new Title { TitleId = 2, Shortcut = "Mgr.", Name = "Magistr", IsActive = true },
                new Title { TitleId = 3, Shortcut = "Ing.", Name = "Inzenyr", IsActive = true },
                new Title { TitleId = 4, Shortcut = "PhDr.", Name = "Doktor filozofie", IsActive = true },
                new Title { TitleId = 5, Shortcut = "JUDr.", Name = "Doktor prav", IsActive = true },
                new Title { TitleId = 6, Shortcut = "RNDr.", Name = "Doktor prirodnich ved", IsActive = true },
                new Title { TitleId = 7, Shortcut = "Ph.D.", Name = "Doktor filozofie", IsActive = true },
                new Title { TitleId = 8, Shortcut = "Th.D.", Name = "Doktor teologie", IsActive = true },
                new Title { TitleId = 9, Shortcut = "MBA", Name = "Magistersky titul obchodni administrativy", IsActive = true },
                new Title { TitleId = 10, Shortcut = "LL.M.", Name = "Magistr prav", IsActive = true }
            );

            modelBuilder.Entity<StudentField>().HasData(
                new StudentField { StudentFieldId = 1, Name = "Anglicky jazyk", IsActive = true },
                new StudentField { StudentFieldId = 2, Name = "Databaze", IsActive = true },
                new StudentField { StudentFieldId = 3, Name = "Ekonomika", IsActive = true },
                new StudentField { StudentFieldId = 4, Name = "Elektrotechnika", IsActive = true },
                new StudentField { StudentFieldId = 5, Name = "Fyzika", IsActive = true },
                new StudentField { StudentFieldId = 6, Name = "Kyberbezpecnost", IsActive = true },
                new StudentField { StudentFieldId = 7, Name = "Matematicky seminar", IsActive = true },
                new StudentField { StudentFieldId = 8, Name = "Matematika", IsActive = true },
                new StudentField { StudentFieldId = 9, Name = "Operacni systemy", IsActive = true },
                new StudentField { StudentFieldId = 10, Name = "Praxe", IsActive = true },
                new StudentField { StudentFieldId = 11, Name = "Programovani a vyvoj aplikaci", IsActive = true },
                new StudentField { StudentFieldId = 12, Name = "Projekty", IsActive = true },
                new StudentField { StudentFieldId = 13, Name = "Telesna vychova", IsActive = true },
                new StudentField { StudentFieldId = 14, Name = "Cesky jazyk a literatura", IsActive = true },
                new StudentField { StudentFieldId = 15, Name = "Zaklady elektrotechniky", IsActive = true },
                new StudentField { StudentFieldId = 16, Name = "Materialy a technologie", IsActive = true },
                new StudentField { StudentFieldId = 17, Name = "Informacni a komunikacni technologie", IsActive = true },
                new StudentField { StudentFieldId = 18, Name = "Nauka o spolecnosti", IsActive = true },
                new StudentField { StudentFieldId = 19, Name = "Odborny vycvik", IsActive = true },
                new StudentField { StudentFieldId = 20, Name = "Elektricke stroje a pristroje", IsActive = true },
                new StudentField { StudentFieldId = 21, Name = "Technicka dokumentace", IsActive = true },
                new StudentField { StudentFieldId = 22, Name = "Dejepis", IsActive = true },
                new StudentField { StudentFieldId = 23, Name = "Aplikacni software", IsActive = true },
                new StudentField { StudentFieldId = 24, Name = "Webove aplikace", IsActive = true },
                new StudentField { StudentFieldId = 25, Name = "Zaklady prirodnich ved", IsActive = true },
                new StudentField { StudentFieldId = 26, Name = "Algoritmizace", IsActive = true },
                new StudentField { StudentFieldId = 27, Name = "Datove site", IsActive = true },
                new StudentField { StudentFieldId = 28, Name = "Logistika", IsActive = true },
                new StudentField { StudentFieldId = 29, Name = "Doprava", IsActive = true },
                new StudentField { StudentFieldId = 30, Name = "Nemecky jazyk", IsActive = true },
                new StudentField { StudentFieldId = 31, Name = "Pisemna a elektronicka komunikace", IsActive = true },
                new StudentField { StudentFieldId = 32, Name = "Obcanska nauka", IsActive = true },
                new StudentField { StudentFieldId = 33, Name = "Automatizace", IsActive = true },
                new StudentField { StudentFieldId = 34, Name = "Elektronika", IsActive = true },
                new StudentField { StudentFieldId = 35, Name = "Elektrotechnicka mereni", IsActive = true },
                new StudentField { StudentFieldId = 36, Name = "Herni vyvoj", IsActive = true },
                new StudentField { StudentFieldId = 37, Name = "Marketing a management", IsActive = true },
                new StudentField { StudentFieldId = 38, Name = "Ucetnictvi", IsActive = true },
                new StudentField { StudentFieldId = 39, Name = "Webove technologie", IsActive = true },
                new StudentField { StudentFieldId = 40, Name = "Zemepis", IsActive = true },
                new StudentField { StudentFieldId = 41, Name = "Strojnictvi", IsActive = true },
                new StudentField { StudentFieldId = 42, Name = "Programovani", IsActive = true },
                new StudentField { StudentFieldId = 43, Name = "Elektronika a sdelovaci technika", IsActive = true },
                new StudentField { StudentFieldId = 44, Name = "Cislicova technika", IsActive = true },
                new StudentField { StudentFieldId = 45, Name = "Automatizace", IsActive = true },
                new StudentField { StudentFieldId = 46, Name = "Mikroprocesorova technika", IsActive = true },
                new StudentField { StudentFieldId = 47, Name = "Technicke kresleni", IsActive = true }
            );

            modelBuilder.Entity<QuestionType>().HasData(
                new QuestionType { QuestionTypeId = 1, Name = "Vyber z moznosti", IsActive = true },
                new QuestionType { QuestionTypeId = 2, Name = "Uzavrena otazka s obrazky", IsActive = true }
            );

            // -- Tridy --
            modelBuilder.Entity<Classes>().HasData(
                new Classes { ClassesId = 1, Name = "1.A", StartFrom = 2025, EndTo = 2029, IsActive = true },
                new Classes { ClassesId = 2, Name = "1.B", StartFrom = 2025, EndTo = 2029, IsActive = true },
                new Classes { ClassesId = 3, Name = "2.A", StartFrom = 2024, EndTo = 2028, IsActive = true },
                new Classes { ClassesId = 4, Name = "2.B", StartFrom = 2024, EndTo = 2028, IsActive = true },
                new Classes { ClassesId = 5, Name = "3.A", StartFrom = 2023, EndTo = 2027, IsActive = true },
                new Classes { ClassesId = 6, Name = "4.A", StartFrom = 2022, EndTo = 2026, IsActive = false } // absolventi
            );

            // -- Predmety prirazene tridam --
            modelBuilder.Entity<ClassesFields>().HasData(
                // 1.A (IT): PVA, Algoritmizace, Matematika, Anglictina, ICT
                new ClassesFields { ClassesId = 1, StudentFieldId = 11 },
                new ClassesFields { ClassesId = 1, StudentFieldId = 26 },
                new ClassesFields { ClassesId = 1, StudentFieldId = 8 },
                new ClassesFields { ClassesId = 1, StudentFieldId = 1 },
                new ClassesFields { ClassesId = 1, StudentFieldId = 17 },
                // 1.B (Elektro): Elektrotechnika, ZE, Matematika, Fyzika, Anglictina
                new ClassesFields { ClassesId = 2, StudentFieldId = 4 },
                new ClassesFields { ClassesId = 2, StudentFieldId = 15 },
                new ClassesFields { ClassesId = 2, StudentFieldId = 8 },
                new ClassesFields { ClassesId = 2, StudentFieldId = 5 },
                new ClassesFields { ClassesId = 2, StudentFieldId = 1 },
                // 2.A (IT): Databaze, PVA, Webove aplikace, OS, Matematika
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
                // 3.A (IT): Kyberbezpecnost, Datove site, Programovani, Projekty
                new ClassesFields { ClassesId = 5, StudentFieldId = 6 },
                new ClassesFields { ClassesId = 5, StudentFieldId = 27 },
                new ClassesFields { ClassesId = 5, StudentFieldId = 42 },
                new ClassesFields { ClassesId = 5, StudentFieldId = 12 },
                // 4.A (IT): Projekty, Praxe, Programovani, Webove technologie
                new ClassesFields { ClassesId = 6, StudentFieldId = 12 },
                new ClassesFields { ClassesId = 6, StudentFieldId = 10 },
                new ClassesFields { ClassesId = 6, StudentFieldId = 42 },
                new ClassesFields { ClassesId = 6, StudentFieldId = 39 }
            );

            // -- Studenti (24 studentu, 4 na tridu) --
            modelBuilder.Entity<Student>().HasData(
                // 1.A
                new Student { StudentId = 1, FirstName = "Jakub", LastName = "Horak", IsActive = true },
                new Student { StudentId = 2, FirstName = "Tereza", LastName = "Markova", IsActive = true },
                new Student { StudentId = 3, FirstName = "Martin", LastName = "Jelinek", IsActive = true },
                new Student { StudentId = 4, FirstName = "Lucie", LastName = "Pokorna", IsActive = true },
                // 1.B
                new Student { StudentId = 5, FirstName = "David", LastName = "Ruzicka", IsActive = true },
                new Student { StudentId = 6, FirstName = "Anna", LastName = "Benesova", IsActive = true },
                new Student { StudentId = 7, FirstName = "Ondrej", LastName = "Fiala", IsActive = true },
                new Student { StudentId = 8, FirstName = "Karolina", LastName = "Stastna", IsActive = true },
                // 2.A
                new Student { StudentId = 9, FirstName = "Vojtech", LastName = "Kucera", IsActive = true },
                new Student { StudentId = 10, FirstName = "Eliska", LastName = "Vesela", IsActive = true },
                new Student { StudentId = 11, FirstName = "Matej", LastName = "Marek", IsActive = true },
                new Student { StudentId = 12, FirstName = "Natalie", LastName = "Kopecka", IsActive = false },
                // 2.B
                new Student { StudentId = 13, FirstName = "Stepan", LastName = "Havlicek", IsActive = true },
                new Student { StudentId = 14, FirstName = "Michaela", LastName = "Vlckova", IsActive = true },
                new Student { StudentId = 15, FirstName = "Adam", LastName = "Bartos", IsActive = true },
                new Student { StudentId = 16, FirstName = "Barbora", LastName = "Urbanova", IsActive = true },
                // 3.A
                new Student { StudentId = 17, FirstName = "Daniel", LastName = "Blazek", IsActive = true },
                new Student { StudentId = 18, FirstName = "Kristyna", LastName = "Sedlackova", IsActive = true },
                new Student { StudentId = 19, FirstName = "Filip", LastName = "Kratochvil", IsActive = true },
                new Student { StudentId = 20, FirstName = "Simona", LastName = "Nemcova", IsActive = true },
                // 4.A
                new Student { StudentId = 21, FirstName = "Lukas", LastName = "Pospisil", IsActive = true },
                new Student { StudentId = 22, FirstName = "Veronika", LastName = "Holubova", IsActive = true },
                new Student { StudentId = 23, FirstName = "Dominik", LastName = "Simek", IsActive = true },
                new Student { StudentId = 24, FirstName = "Klara", LastName = "Dostalova", IsActive = true }
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

            // -- Otazky (12 otazek, ruzne predmety a tvurci) --
            modelBuilder.Entity<Question>().HasData(
                // Programovani (FieldId=11, Creator=4 Svobodova)
                new Question { QuestionId = 1, Header = "Co je to promenna?", Description = "Zakladni koncept programovani — vysvetlete, co znamena pojem promenna.", QuestionTypeId = 1, FieldId = 11, CreatorId = 4, IsActive = true },
                new Question { QuestionId = 2, Header = "Ktery datovy typ je celociselny v C#?", Description = "Datove typy v jazyce C# — vyberte spravny celociselny typ.", QuestionTypeId = 1, FieldId = 11, CreatorId = 4, IsActive = true },
                new Question { QuestionId = 3, Header = "Co dela prikaz 'if'?", Description = "Ridici struktury v programovani — podminene vetveni.", QuestionTypeId = 1, FieldId = 11, CreatorId = 4, IsActive = true },
                new Question { QuestionId = 4, Header = "Co je to cyklus 'for'?", Description = "Cykly v programovani — opakovani bloku kodu.", QuestionTypeId = 1, FieldId = 11, CreatorId = 7, IsActive = true },
                // Databaze (FieldId=2, Creator=7 Prochazka)
                new Question { QuestionId = 5, Header = "Co znamena zkratka SQL?", Description = "Zaklady databazovych jazyku — puvod zkratky SQL.", QuestionTypeId = 1, FieldId = 2, CreatorId = 7, IsActive = true },
                new Question { QuestionId = 6, Header = "Ktery prikaz slouzi k vyberu dat?", Description = "SQL prikazy pro cteni dat z databaze.", QuestionTypeId = 1, FieldId = 2, CreatorId = 7, IsActive = true },
                new Question { QuestionId = 7, Header = "Co je primarni klic?", Description = "Integritni omezeni v relacnich databazich.", QuestionTypeId = 1, FieldId = 2, CreatorId = 7, IsActive = true },
                new Question { QuestionId = 8, Header = "Co dela prikaz JOIN?", Description = "Spojovani tabulek v SQL dotazech.", QuestionTypeId = 1, FieldId = 2, CreatorId = 4, IsActive = true },
                // Kyberbezpecnost (FieldId=6, Creator=4 Svobodova)
                new Question { QuestionId = 9, Header = "Co je phishing?", Description = "Socialni inzenyrstvi — rozpoznavani phishingovych utoku.", QuestionTypeId = 1, FieldId = 6, CreatorId = 4, IsActive = true },
                new Question { QuestionId = 10, Header = "Co je to firewall?", Description = "Sitova bezpecnost — ochrana perimetru site.", QuestionTypeId = 1, FieldId = 6, CreatorId = 4, IsActive = true },
                // Matematika (FieldId=8, Creator=1 Admin)
                new Question { QuestionId = 11, Header = "Kolik je 2^3?", Description = "Mocniny — vypocet treti mocniny cisla 2.", QuestionTypeId = 1, FieldId = 8, CreatorId = 1, IsActive = true },
                // Neaktivni otazka pro testovani filtru
                new Question { QuestionId = 12, Header = "Zastarala otazka", Description = "Tato otazka byla deaktivovana a nemela by se zobrazovat v testech.", QuestionTypeId = 1, FieldId = 11, CreatorId = 4, IsActive = false }
            );

            // -- Moznosti odpovedi (4 na otazku, 1 spravna) --
            modelBuilder.Entity<QuestionOption>().HasData(
                // Q1: Co je to promenna?
                new QuestionOption { QuestionOptionId = 1, QuestionId = 1, Text = "Funkce pro vypocet", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 2, QuestionId = 1, Text = "Pojmenovane misto v pameti pro ulozeni hodnoty", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 3, QuestionId = 1, Text = "Typ souboru", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 4, QuestionId = 1, Text = "Prikaz pro vystup na obrazovku", IsCorrect = false, ImageKey = "" },
                // Q2: Celociselny typ v C#?
                new QuestionOption { QuestionOptionId = 5, QuestionId = 2, Text = "number", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 6, QuestionId = 2, Text = "text", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 7, QuestionId = 2, Text = "int", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 8, QuestionId = 2, Text = "letra", IsCorrect = false, ImageKey = "" },
                // Q3: Co dela if?
                new QuestionOption { QuestionOptionId = 9, QuestionId = 3, Text = "Opakuje blok kodu", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 10, QuestionId = 3, Text = "Definuje novou funkci", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 11, QuestionId = 3, Text = "Vykona kod pouze pokud je podminka pravdiva", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 12, QuestionId = 3, Text = "Ukonci program", IsCorrect = false, ImageKey = "" },
                // Q4: Co je cyklus for?
                new QuestionOption { QuestionOptionId = 13, QuestionId = 4, Text = "Podmineny prikaz", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 14, QuestionId = 4, Text = "Cyklus s predem danym poctem opakovani", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 15, QuestionId = 4, Text = "Deklarace promenne", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 16, QuestionId = 4, Text = "Import knihovny", IsCorrect = false, ImageKey = "" },
                // Q5: Co znamena SQL?
                new QuestionOption { QuestionOptionId = 17, QuestionId = 5, Text = "Standard Query Language", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 18, QuestionId = 5, Text = "Structured Query Language", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 19, QuestionId = 5, Text = "System Query Logic", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 20, QuestionId = 5, Text = "Simple Question Language", IsCorrect = false, ImageKey = "" },
                // Q6: Prikaz pro vyber dat?
                new QuestionOption { QuestionOptionId = 21, QuestionId = 6, Text = "INSERT", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 22, QuestionId = 6, Text = "UPDATE", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 23, QuestionId = 6, Text = "SELECT", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 24, QuestionId = 6, Text = "DELETE", IsCorrect = false, ImageKey = "" },
                // Q7: Co je primarni klic?
                new QuestionOption { QuestionOptionId = 25, QuestionId = 7, Text = "Heslo do databaze", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 26, QuestionId = 7, Text = "Unikatni identifikator zaznamu v tabulce", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 27, QuestionId = 7, Text = "Nazev tabulky", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 28, QuestionId = 7, Text = "Typ sloupce", IsCorrect = false, ImageKey = "" },
                // Q8: Co dela JOIN?
                new QuestionOption { QuestionOptionId = 29, QuestionId = 8, Text = "Maze data ze dvou tabulek", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 30, QuestionId = 8, Text = "Spojuje data z vice tabulek na zaklade podminky", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 31, QuestionId = 8, Text = "Vytvari novou tabulku", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 32, QuestionId = 8, Text = "Radi vysledky vzestupne", IsCorrect = false, ImageKey = "" },
                // Q9: Co je phishing?
                new QuestionOption { QuestionOptionId = 33, QuestionId = 9, Text = "Antivirovy program", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 34, QuestionId = 9, Text = "Podvodny pokus o ziskani citlivych udaju", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 35, QuestionId = 9, Text = "Sifrovaci algoritmus", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 36, QuestionId = 9, Text = "Typ sitoveho kabelu", IsCorrect = false, ImageKey = "" },
                // Q10: Co je firewall?
                new QuestionOption { QuestionOptionId = 37, QuestionId = 10, Text = "Hardware pro tisk", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 38, QuestionId = 10, Text = "Zarizeni/software filtrujici sitovy provoz", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 39, QuestionId = 10, Text = "Typ operacniho systemu", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 40, QuestionId = 10, Text = "Programovaci jazyk", IsCorrect = false, ImageKey = "" },
                // Q11: Kolik je 2^3?
                new QuestionOption { QuestionOptionId = 41, QuestionId = 11, Text = "6", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 42, QuestionId = 11, Text = "8", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 43, QuestionId = 11, Text = "9", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 44, QuestionId = 11, Text = "4", IsCorrect = false, ImageKey = "" },
                // Q12: Zastarala (neaktivni)
                new QuestionOption { QuestionOptionId = 45, QuestionId = 12, Text = "Odpoved A", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 46, QuestionId = 12, Text = "Odpoved B", IsCorrect = true, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 47, QuestionId = 12, Text = "Odpoved C", IsCorrect = false, ImageKey = "" },
                new QuestionOption { QuestionOptionId = 48, QuestionId = 12, Text = "Odpoved D", IsCorrect = false, ImageKey = "" }
            );

            // -- Testy (3 testy s JSON snapshoty otazek) --
            modelBuilder.Entity<Test>().HasData(
                new Test
                {
                    TestId = 1,
                    Name = "PVA — Zaklady programovani",
                    CreatorId = 4, // Svobodova
                    StudentFieldId = 11,
                    TimeLimitMinutes = 15,
                    ShuffleQuestions = true,
                    IsActive = true,
                    QuestionSnapshot = """[{"QuestionId":1,"Header":"Co je to promenna?","Description":"Zakladni koncept programovani.","QuestionType":"Vyber z moznosti","Options":[{"Text":"Funkce pro vypocet","ImageKey":null,"IsCorrect":false},{"Text":"Pojmenovane misto v pameti pro ulozeni hodnoty","ImageKey":null,"IsCorrect":true},{"Text":"Typ souboru","ImageKey":null,"IsCorrect":false},{"Text":"Prikaz pro vystup na obrazovku","ImageKey":null,"IsCorrect":false}]},{"QuestionId":2,"Header":"Ktery datovy typ je celociselny v C#?","Description":"Datove typy v jazyce C#.","QuestionType":"Vyber z moznosti","Options":[{"Text":"number","ImageKey":null,"IsCorrect":false},{"Text":"text","ImageKey":null,"IsCorrect":false},{"Text":"int","ImageKey":null,"IsCorrect":true},{"Text":"letra","ImageKey":null,"IsCorrect":false}]},{"QuestionId":3,"Header":"Co dela prikaz 'if'?","Description":"Ridici struktury.","QuestionType":"Vyber z moznosti","Options":[{"Text":"Opakuje blok kodu","ImageKey":null,"IsCorrect":false},{"Text":"Definuje novou funkci","ImageKey":null,"IsCorrect":false},{"Text":"Vykona kod pouze pokud je podminka pravdiva","ImageKey":null,"IsCorrect":true},{"Text":"Ukonci program","ImageKey":null,"IsCorrect":false}]},{"QuestionId":4,"Header":"Co je to cyklus 'for'?","Description":"Cykly v programovani.","QuestionType":"Vyber z moznosti","Options":[{"Text":"Podmineny prikaz","ImageKey":null,"IsCorrect":false},{"Text":"Cyklus s predem danym poctem opakovani","ImageKey":null,"IsCorrect":true},{"Text":"Deklarace promenne","ImageKey":null,"IsCorrect":false},{"Text":"Import knihovny","ImageKey":null,"IsCorrect":false}]}]"""
                },
                new Test
                {
                    TestId = 2,
                    Name = "Databaze — SQL zaklady",
                    CreatorId = 7, // Prochazka
                    StudentFieldId = 2,
                    TimeLimitMinutes = 20,
                    ShuffleQuestions = false,
                    IsActive = true,
                    QuestionSnapshot = """[{"QuestionId":5,"Header":"Co znamena zkratka SQL?","Description":"Zaklady databazovych jazyku.","QuestionType":"Vyber z moznosti","Options":[{"Text":"Standard Query Language","ImageKey":null,"IsCorrect":false},{"Text":"Structured Query Language","ImageKey":null,"IsCorrect":true},{"Text":"System Query Logic","ImageKey":null,"IsCorrect":false},{"Text":"Simple Question Language","ImageKey":null,"IsCorrect":false}]},{"QuestionId":6,"Header":"Ktery prikaz slouzi k vyberu dat?","Description":"SQL prikazy.","QuestionType":"Vyber z moznosti","Options":[{"Text":"INSERT","ImageKey":null,"IsCorrect":false},{"Text":"UPDATE","ImageKey":null,"IsCorrect":false},{"Text":"SELECT","ImageKey":null,"IsCorrect":true},{"Text":"DELETE","ImageKey":null,"IsCorrect":false}]},{"QuestionId":7,"Header":"Co je primarni klic?","Description":"Integritni omezeni.","QuestionType":"Vyber z moznosti","Options":[{"Text":"Heslo do databaze","ImageKey":null,"IsCorrect":false},{"Text":"Unikatni identifikator zaznamu v tabulce","ImageKey":null,"IsCorrect":true},{"Text":"Nazev tabulky","ImageKey":null,"IsCorrect":false},{"Text":"Typ sloupce","ImageKey":null,"IsCorrect":false}]},{"QuestionId":8,"Header":"Co dela prikaz JOIN?","Description":"Spojovani tabulek.","QuestionType":"Vyber z moznosti","Options":[{"Text":"Maze data ze dvou tabulek","ImageKey":null,"IsCorrect":false},{"Text":"Spojuje data z vice tabulek na zaklade podminky","ImageKey":null,"IsCorrect":true},{"Text":"Vytvari novou tabulku","ImageKey":null,"IsCorrect":false},{"Text":"Radi vysledky vzestupne","ImageKey":null,"IsCorrect":false}]}]"""
                },
                new Test
                {
                    TestId = 3,
                    Name = "Kyberbezpecnost — Uvod",
                    CreatorId = 4, // Svobodova
                    StudentFieldId = 6,
                    TimeLimitMinutes = null,
                    ShuffleQuestions = true,
                    IsActive = false,
                    QuestionSnapshot = """[{"QuestionId":9,"Header":"Co je phishing?","Description":"Socialni inzenyrstvi.","QuestionType":"Vyber z moznosti","Options":[{"Text":"Antivirovy program","ImageKey":null,"IsCorrect":false},{"Text":"Podvodny pokus o ziskani citlivych udaju","ImageKey":null,"IsCorrect":true},{"Text":"Sifrovaci algoritmus","ImageKey":null,"IsCorrect":false},{"Text":"Typ sitoveho kabelu","ImageKey":null,"IsCorrect":false}]},{"QuestionId":10,"Header":"Co je to firewall?","Description":"Sitova bezpecnost.","QuestionType":"Vyber z moznosti","Options":[{"Text":"Hardware pro tisk","ImageKey":null,"IsCorrect":false},{"Text":"Zarizeni/software filtrujici sitovy provoz","ImageKey":null,"IsCorrect":true},{"Text":"Typ operacniho systemu","ImageKey":null,"IsCorrect":false},{"Text":"Programovaci jazyk","ImageKey":null,"IsCorrect":false}]}]"""
                }
            );

            modelBuilder.Entity<StudentTest>().HasData(
                // Test 1 (PVA) -> studenti z 1.A
                new StudentTest
                {
                    StudentId = 1, TestId = 1, LoginId = "PVA-HOR-001",
                    StartedAt = new DateTime(2026, 3, 10, 8, 0, 0, DateTimeKind.Utc),
                    FinishedAt = new DateTime(2026, 3, 10, 8, 12, 0, DateTimeKind.Utc),
                    ResultSnapshot = """{"Answers":[{"QuestionId":1,"SelectedOptions":["Pojmenovane misto v pameti pro ulozeni hodnoty"]},{"QuestionId":2,"SelectedOptions":["int"]},{"QuestionId":3,"SelectedOptions":["Vykona kod pouze pokud je podminka pravdiva"]},{"QuestionId":4,"SelectedOptions":["Cyklus s predem danym poctem opakovani"]}],"CurrentQuestionIndex":3}"""
                },
                new StudentTest
                {
                    StudentId = 2, TestId = 1, LoginId = "PVA-MAR-002",
                    StartedAt = new DateTime(2026, 3, 10, 8, 0, 0, DateTimeKind.Utc),
                    FinishedAt = DateTime.MinValue,
                    ResultSnapshot = """{"Answers":[{"QuestionId":1,"SelectedOptions":["Pojmenovane misto v pameti pro ulozeni hodnoty"]},{"QuestionId":2,"SelectedOptions":["text"]}],"CurrentQuestionIndex":2}"""
                },
                new StudentTest
                {
                    StudentId = 3, TestId = 1, LoginId = "PVA-JEL-003",
                    StartedAt = DateTime.MinValue,
                    FinishedAt = DateTime.MinValue,
                    ResultSnapshot = """{"Answers":[],"CurrentQuestionIndex":0}"""
                },
                // Test 2 (SQL) -> studenti z 2.A
                new StudentTest
                {
                    StudentId = 9, TestId = 2, LoginId = "SQL-KUC-009",
                    StartedAt = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
                    FinishedAt = new DateTime(2026, 3, 12, 10, 18, 0, DateTimeKind.Utc),
                    ResultSnapshot = """{"Answers":[{"QuestionId":5,"SelectedOptions":["Structured Query Language"]},{"QuestionId":6,"SelectedOptions":["SELECT"]},{"QuestionId":7,"SelectedOptions":["Nazev tabulky"]},{"QuestionId":8,"SelectedOptions":["Spojuje data z vice tabulek na zaklade podminky"]}],"CurrentQuestionIndex":3}"""
                },
                new StudentTest
                {
                    StudentId = 10, TestId = 2, LoginId = "SQL-VES-010",
                    StartedAt = new DateTime(2026, 3, 12, 10, 0, 0, DateTimeKind.Utc),
                    FinishedAt = new DateTime(2026, 3, 12, 10, 15, 0, DateTimeKind.Utc),
                    ResultSnapshot = """{"Answers":[{"QuestionId":5,"SelectedOptions":["Structured Query Language"]},{"QuestionId":6,"SelectedOptions":["SELECT"]},{"QuestionId":7,"SelectedOptions":["Unikatni identifikator zaznamu v tabulce"]},{"QuestionId":8,"SelectedOptions":["Spojuje data z vice tabulek na zaklade podminky"]}],"CurrentQuestionIndex":3}"""
                },
                new StudentTest
                {
                    StudentId = 11, TestId = 2, LoginId = "SQL-MAR-011",
                    StartedAt = DateTime.MinValue,
                    FinishedAt = DateTime.MinValue,
                    ResultSnapshot = """{"Answers":[],"CurrentQuestionIndex":0}"""
                },
                // Test 3 (Kyber) -> studenti z 3.A
                new StudentTest
                {
                    StudentId = 17, TestId = 3, LoginId = "KYB-BLA-017",
                    StartedAt = new DateTime(2026, 2, 20, 9, 0, 0, DateTimeKind.Utc),
                    FinishedAt = new DateTime(2026, 2, 20, 9, 8, 0, DateTimeKind.Utc),
                    ResultSnapshot = """{"Answers":[{"QuestionId":9,"SelectedOptions":["Podvodny pokus o ziskani citlivych udaju"]},{"QuestionId":10,"SelectedOptions":["Zarizeni/software filtrujici sitovy provoz"]}],"CurrentQuestionIndex":1}"""
                },
                new StudentTest
                {
                    StudentId = 18, TestId = 3, LoginId = "KYB-SED-018",
                    StartedAt = new DateTime(2026, 2, 20, 9, 0, 0, DateTimeKind.Utc),
                    FinishedAt = new DateTime(2026, 2, 20, 9, 5, 0, DateTimeKind.Utc),
                    ResultSnapshot = """{"Answers":[{"QuestionId":9,"SelectedOptions":["Sifrovaci algoritmus"]},{"QuestionId":10,"SelectedOptions":["Zarizeni/software filtrujici sitovy provoz"]}],"CurrentQuestionIndex":1}"""
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
