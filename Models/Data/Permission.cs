namespace SPSUL.Models.Data
{
    public class Permission
    {
        public int PermissionId { get; set; }
        public required string Name { get; set; }
        public bool IsActive { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }

    }
}
