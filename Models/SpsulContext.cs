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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>(e =>
            {
                e.HasKey(e => e.TeacherId);

                e.Property(e => e.LastName).HasMaxLength(16);
                e.Property(e => e.FirstName).HasMaxLength(16);
                e.Property(e => e.NickName).HasMaxLength(32);
                e.Property(e => e.Password).HasMaxLength(64);

                e.HasOne(e => e.Role).WithMany(e => e.Teachers).HasForeignKey(e => e.RoleId);
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

                e.Property(e => e.Shortcut).HasMaxLength(8);

                e.Property(e => e.Name).HasMaxLength(48);
            });

            modelBuilder.Entity<Role>(e =>
            {
                e.HasKey(e => e.RoleId);

                e.Property(e => e.Name).HasMaxLength(32);
            });

            modelBuilder.Entity<Permission>(e =>
            {
                e.HasKey(e => e.PermissionId);

                e.Property(e => e.Name).HasMaxLength(16);
            });

            modelBuilder.Entity<RolePermission>(e =>
            {
                e.HasKey(e => new { e.PermissionId, e.RoleId });

                e.HasOne(e => e.Permissions).WithMany(e => e.RolePermissions).HasForeignKey(e => e.PermissionId);

                e.HasOne(e => e.Roles).WithMany(e => e.RolePermissions).HasForeignKey(e => e.RoleId);
            });

            modelBuilder.Entity<Session>(e =>
            {
                e.Property(e => e.SessionId).HasMaxLength(64);
            });


            base.OnModelCreating(modelBuilder);
        }
    }
}
