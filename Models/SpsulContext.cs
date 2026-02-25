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


            ////Default datas
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { TeacherId = 1, FirstName = "Admin", LastName = "Admin", NickName = "Admin", PasswordHash = "$2b$10$jErDDvlTESkhHfdiHuRFte9ojuRZNZST.gskJ4PVgp6h6q0VGmVxS" }, // Admin-admin1234 Role[Admin]
                new Teacher { TeacherId = 2, FirstName = "Filip", LastName = "Eder", NickName = "FilipEder", PasswordHash = "$2b$10$4l/ga1u8GL4dxznTb/t73eiKqRRfMKIsLpi8bCQQxkGtmnEX64NoS" }, // FilipEder-heslo1234 Role[Hledic]
                new Teacher { TeacherId = 3, FirstName = "Petr", LastName = "Novák", NickName = "PetrNovak", PasswordHash = "$2b$10$YqPz7WEHhmjRpRuFqaVPVu505tO1z4KwGVnnj3T3J0S9SEnZMrZSG" } // PetrNovak-PetrNovak1234 Role[Bez]
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "Administrátor", Description = "Oprávnění uděluje plnou kontrolu nad systémem – správce může vytvářet, upravovat i mazat všechny účty.", IsActive = true},
                new Role { RoleId = 2, Name = "Tvůrce", Description = "Oprávnění uděluje možnost vytváření, aktualizování a čtení všech systému v aplikaci, krom učitelů.", IsActive = true },
                new Role { RoleId = 3, Name = "Testátor", Description = "Oprávnění uděluje možnost všechny operace pro systém testů.", IsActive = true },
                new Role { RoleId = 4, Name = "Učitelátor", Description = "Oprávnění uděluje možnost všechny operace pro systém učitelů.", IsActive = true },
                new Role { RoleId = 5, Name = "Studentátor", Description = "Oprávnění uděluje možnost všechny operace pro systém studentů.", IsActive = true },
                new Role { RoleId = 6, Name = "Hledič", Description = "Oprávnění uděluje pohled na všechny systémy.", IsActive = true }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionId = 1, Name = "All Permissions", IsActive = true},
                new Permission { PermissionId = 2, Name = "CURD", IsActive = true },
                new Permission { PermissionId = 3, Name = "CRUD Test", IsActive = true},
                new Permission { PermissionId = 4, Name = "CRUD Teacher", IsActive = true},
                new Permission { PermissionId = 5, Name = "CRUD Student", IsActive = true},
                new Permission { PermissionId = 6, Name = "View", IsActive = true}
            );

            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { RoleId = 1, PermissionId = 1 },
                new RolePermission { RoleId = 2, PermissionId = 2 },
                new RolePermission { RoleId = 3, PermissionId = 3 },
                new RolePermission { RoleId = 4, PermissionId = 4 },
                new RolePermission { RoleId = 5, PermissionId = 5 },
                new RolePermission { RoleId = 6, PermissionId = 6}
            );

            modelBuilder.Entity<TeacherRole>().HasData(
                new TeacherRole { TeacherId = 1, RoleId = 1 },
                new TeacherRole { TeacherId = 2, RoleId = 6 }
            );

            modelBuilder.Entity<Title>().HasData(
                new Title { TitleId = 1 , Shortcut = "Bc.", Name = "Bakalář", IsActive = true },
                new Title { TitleId = 2 , Shortcut = "Mgr.", Name = "Magistr", IsActive = true },
                new Title { TitleId = 3 , Shortcut = "Ing.", Name = "Inženýr", IsActive = true },
                new Title { TitleId = 4 , Shortcut = "PhDr.", Name = "Doktor filozofie", IsActive = true },
                new Title { TitleId = 5 , Shortcut = "JUDr.", Name = "Doktor práv", IsActive = true },
                new Title { TitleId = 6 , Shortcut = "RNDr.", Name = "Doktor přírodních věd", IsActive = true },
                new Title { TitleId = 7 , Shortcut = "Ph.D.", Name = "Doktor filozofie", IsActive = true },
                new Title { TitleId = 8 , Shortcut = "Th.D.", Name = "Doktor teologie", IsActive = true },
                new Title { TitleId = 9 , Shortcut = "MBA", Name = "Magisterský titul obchodní administrativy", IsActive = true },
                new Title { TitleId = 10 , Shortcut = "LL.M.", Name = "Magistr práv", IsActive = true }
            );

            modelBuilder.Entity<StudentField>().HasData(
                new StudentField { StudentFieldId = 1, Name = "Anglický jazyk", IsActive = true },
                new StudentField { StudentFieldId = 2, Name = "Databáze", IsActive = true},
                new StudentField { StudentFieldId = 3, Name = "Ekonomika", IsActive = true},
                new StudentField { StudentFieldId = 4, Name = "Elektrotechnika", IsActive = true},
                new StudentField { StudentFieldId = 5, Name = "Fyzika", IsActive = true},
                new StudentField { StudentFieldId = 6, Name = "Kyberbezpečnost", IsActive = true},
                new StudentField { StudentFieldId = 7, Name = "Matematický seminář", IsActive = true},
                new StudentField { StudentFieldId = 8, Name = "Matematika", IsActive = true},
                new StudentField { StudentFieldId = 9, Name = "Operační systémy", IsActive = true},
                new StudentField { StudentFieldId = 10, Name = "Praxe", IsActive = true},
                new StudentField { StudentFieldId = 11, Name = "Programování a vývoj aplikací", IsActive = true},
                new StudentField { StudentFieldId = 12, Name = "Projekty", IsActive = true},
                new StudentField { StudentFieldId = 13, Name = "Tělesná výchova", IsActive = true},
                new StudentField { StudentFieldId = 14, Name = "Český jazyk a literatura", IsActive = true},
                new StudentField { StudentFieldId = 15, Name = "Základy elektrotechniky", IsActive = true},
                new StudentField { StudentFieldId = 16, Name = "Materiály a technologie", IsActive = true},
                new StudentField { StudentFieldId = 17, Name = "Informační a komunikační technologie", IsActive = true},
                new StudentField { StudentFieldId = 18, Name = "Nauka o společnosti", IsActive = true},
                new StudentField { StudentFieldId = 19, Name = "Odborný výcvik", IsActive = true},
                new StudentField { StudentFieldId = 20, Name = "Elektrické stroje a přístroje", IsActive = true},
                new StudentField { StudentFieldId = 21, Name = "Technická dokumentace", IsActive = true},
                new StudentField { StudentFieldId = 22, Name = "Dějepis", IsActive = true},
                new StudentField { StudentFieldId = 23, Name = "Aplikační software", IsActive = true},
                new StudentField { StudentFieldId = 24, Name = "Webové aplikace", IsActive = true},
                new StudentField { StudentFieldId = 25, Name = "Základy přírodních věd", IsActive = true},
                new StudentField { StudentFieldId = 26, Name = "Algoritmizace", IsActive = true},
                new StudentField { StudentFieldId = 27, Name = "Datové sítě", IsActive = true},
                new StudentField { StudentFieldId = 28, Name = "Logistika", IsActive = true},
                new StudentField { StudentFieldId = 29, Name = "Doprava", IsActive = true},
                new StudentField { StudentFieldId = 30, Name = "Německý jazyk", IsActive = true},
                new StudentField { StudentFieldId = 31, Name = "Písemná a elektronická komunikace", IsActive = true},
                new StudentField { StudentFieldId = 32, Name = "Občanská nauka", IsActive = true},
                new StudentField { StudentFieldId = 33, Name = "Automatizace", IsActive = true},
                new StudentField { StudentFieldId = 34, Name = "Elektronika", IsActive = true},
                new StudentField { StudentFieldId = 35, Name = "Elektrotechnická měření", IsActive = true},
                new StudentField { StudentFieldId = 36, Name = "Herní vývoj", IsActive = true},
                new StudentField { StudentFieldId = 37, Name = "Marketing a management", IsActive = true},
                new StudentField { StudentFieldId = 38, Name = "Účetnictví", IsActive = true},
                new StudentField { StudentFieldId = 39, Name = "Webové technologie", IsActive = true},
                new StudentField { StudentFieldId = 40, Name = "Zeměpis", IsActive = true},
                new StudentField { StudentFieldId = 41, Name = "Strojnictví", IsActive = true},
                new StudentField { StudentFieldId = 42, Name = "Programování", IsActive = true},
                new StudentField { StudentFieldId = 43, Name = "Elektronika a sdělovací technika", IsActive = true},
                new StudentField { StudentFieldId = 44, Name = "Číslicová technika", IsActive = true},
                new StudentField { StudentFieldId = 45, Name = "Automatizace", IsActive = true},
                new StudentField { StudentFieldId = 46, Name = "Mikroprocesorová technika", IsActive = true},
                new StudentField { StudentFieldId = 47, Name = "Technické kreslení", IsActive = true}
            );
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
