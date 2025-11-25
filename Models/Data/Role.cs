namespace SPSUL.Models.Data
{
    public class Role
    {
        public int RoleId { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Teacher> Teachers { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
