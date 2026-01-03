namespace SPSUL.Models.Data
{
    public class Role
    {
        public int RoleId { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
        public ICollection<TeacherRole> TeacherRoles { get; set; }
    }
}
