using Microsoft.EntityFrameworkCore;
using SPSUL.Models.Data;

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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>(e =>
            {
                e.HasKey(e => e.TeacherId);

                e.Property(e => e.LastName).HasMaxLength(64);
                e.Property(e => e.FirstName).HasMaxLength(64);
                e.Property(e => e.NickName).HasMaxLength(64);
                e.Property(e => e.PasswordHash).HasMaxLength(255);
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
                e.Property(e => e.Name).HasMaxLength(32);
                e.Property(e => e.IsActive).HasDefaultValue(true);
            });

            modelBuilder.Entity<Question>(e =>
            {
                e.HasKey(e => e.QuestionId);
                e.Property(e => e.Header).HasMaxLength(128);
                e.Property(e => e.Description).HasMaxLength(512);
                e.HasOne(e => e.QuestionType).WithMany(e => e.Questions).HasForeignKey(e => e.QuestionTypeId);

            });

            modelBuilder.Entity<QuestionOption>(e =>
            {
                e.HasKey(e => e.QuestionOptionId);
                e.Property(e => e.Text).HasMaxLength(512);
                e.Property(e => e.IsCorrect);
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

            base.OnModelCreating(modelBuilder);
        }
    }
}
