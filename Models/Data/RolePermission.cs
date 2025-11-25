namespace SPSUL.Models.Data
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public Role Roles { get; set; }
        public Permission Permissions { get; set; }
    }
}
