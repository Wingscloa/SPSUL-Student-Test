namespace SPSUL.Models.Data
{
    public class TeacherRole
    {
        public int TeacherId { get; set; }
        public int RoleId { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual Role Role { get; set; } 
    }
}
